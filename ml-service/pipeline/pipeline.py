from __future__ import annotations
from pathlib import Path
import uuid
import logging
from config import AUDIO_TEMP_DIR, DEVICE

def process(audio_in: Path | str, *, do_master: bool = True) -> dict[str, Path | str]:
    """
    Полный RESTORE-конвейер:
      1) удаление щелчков
      2) восстановление пиков
      3) FullSubNet-2 шумоподавление
      4) (пропущено Demucs)
      5) VoiceFixer для вокала
      6) очистка инструментала FullSubNet-2
      7) финальный declip вокала
      8) мастеринговый компрессор/лимитер
      9) ASR + жанровая классификация
    """
    audio_in = Path(audio_in)
    tmp = AUDIO_TEMP_DIR / uuid.uuid4().hex
    tmp.mkdir(parents=True, exist_ok=True)

    # 1) De-click
    from .click_removal       import remove_clicks
    step1 = tmp / "01_declick.wav"
    remove_clicks(audio_in, step1)

    # 2) De-clip
    from .declip              import declip
    step2 = tmp / "02_declipped.wav"
    declip(step1, step2)

    # 3) Denoise FullSubNet-2
    from .fullsubnet2_denoise import denoise_fullsubnet
    step3 = tmp / "03_denoised_fs.wav"
    denoise_fullsubnet(step2, step3, device=DEVICE)

    # 4) Source separation (Demucs) — пропущено из-за нестабильности на старых записях
    logging.info("Skipping Demucs separation; using full mix for both vocals and instrumental.")
    vocals = step3
    inst   = step3

    # 5) VoiceFixer (вокал)
    from .voicefixer_wrapper  import VoiceFixer
    step5 = tmp / "05_voicefixed.wav"
    vf = VoiceFixer(device=DEVICE, mode=0)
    vf.process(vocals, step5)

    # 6) Denoise инструментала FullSubNet-2
    step6 = tmp / "06_inst_denoised.wav"
    denoise_fullsubnet(inst, step6, device=DEVICE)

    # 7) Финальный declip вокала
    step7 = tmp / "07_final_declipped.wav"
    declip(step5, step7)

    # 8) Мастеринг (опционально)
    from .mastering           import spectral_master_full
    if do_master:
        final = tmp / "08_master.wav"
        spectral_master_full(step7, final, target_lufs=-14.0, peak_limit=-0.3)
    else:
        final = step7

    # 9) ASR + жанровая классификация (не роняем пайплайн при ошибке)
    text, category = "", "unknown"
    try:
        from .stt_whisper    import transcribe
        from .classifier     import clf
        text = transcribe(final)
        category = clf.predict(text)
    except Exception as e:
        logging.warning("ASR/genre tagging failed: %s", e)

    return {
        "restored_path":        final.name,
        "text":                 text,
        "category":             category,
        "instrumental_touchup": step6.name,
    }
