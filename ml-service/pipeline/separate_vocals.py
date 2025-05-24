from __future__ import annotations
from pathlib import Path
import torch
from demucs.apply import apply_model
from demucs.audio import AudioFile, save_audio
from demucs.pretrained import get_model

# ленивый синглтон для модели
_MODEL: torch.nn.Module | None = None

def _get_model() -> torch.nn.Module:
    global _MODEL
    if _MODEL is None:
        # модель загрузится и инициализируется только при первом вызове
        _MODEL = get_model("htdemucs").cpu().eval()
    return _MODEL

def separate_vocals(wav_in: Path | str, out_dir: Path | str) -> tuple[Path, Path]:
    """
    Split *wav_in* into vocals / accompaniment using **htdemucs**.
    """
    wav_in = Path(wav_in)
    out_dir = Path(out_dir)
    out_dir.mkdir(parents=True, exist_ok=True)

    # ---------- read audio --------------------------------------------------
    af = AudioFile(wav_in)
    model = _get_model()  # инициализация модели здесь
    wav = af.read(streams=0, samplerate=model.samplerate)[0]
    if hasattr(af, "close"):
        af.close()

    # ---------- нормализация shape -----------------------------------------
    if wav.dim() == 1:
        wav = wav.unsqueeze(0).unsqueeze(0)
    elif wav.dim() == 2:
        wav = wav.unsqueeze(0)
    elif wav.dim() != 3:
        raise ValueError(f"Unexpected tensor shape {wav.shape}")

    # ---------- если моно, делаем стерео ------------------------------------
    if wav.size(1) == 1:
        wav = wav.expand(-1, 2, -1).contiguous()

    # ---------- inference ---------------------------------------------------
    with torch.inference_mode():
        est = apply_model(model, wav, split=True, overlap=0.25)[0]

    # Demucs order: 0 drums, 1 bass, 2 other, 3 vocals
    vocals = est[3]
    accompaniment = est[0] + est[1] + est[2]

    # ---------- save --------------------------------------------------------
    vocals_path = out_dir / "vocals.wav"
    inst_path   = out_dir / "inst.wav"

    save_audio(vocals,        vocals_path, samplerate=model.samplerate)
    save_audio(accompaniment, inst_path,   samplerate=model.samplerate)

    return vocals_path, inst_path
