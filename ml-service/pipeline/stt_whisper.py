from pathlib import Path
import whisper

_model = None

def _load():
    global _model
    if _model is None:
        _model = whisper.load_model("large-v2", download_root="~/.cache/whisper")
    return _model

def transcribe(wav_path: Path, language="ru") -> str:
    model = _load()
    out   = model.transcribe(str(wav_path), language=language, fp16=False)
    return out["text"].strip()
