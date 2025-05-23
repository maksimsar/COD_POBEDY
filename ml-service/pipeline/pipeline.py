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
      3) Удаление винилового шума
      4) FullSubNet-2 шумоподавление
      5) VoiceFixer для вокала
      6) мастеринговый компрессор/лимитер
      7) ASR + жанровая классификация
    """
    audio_in = Path(audio_in)
    tmp = AUDIO_TEMP_DIR / uuid.uuid4().hex
    tmp.mkdir(parents=True, exist_ok=True)

        # 1) De-click
    from .click_removal import remove_clicks
    step1 = tmp / "01_declick.wav"
    remove_clicks(audio_in, step1)

    # 2) De-clip
    from .declip import declip
    step2 = tmp / "02_declipped.wav"
    declip(step1, step2)

    # 3) FullSubNet-2 шумоподавление
    from .fullsubnet2_denoise import denoise_fullsubnet
    step3 = tmp / "03_denoised_fs.wav"
    denoise_fullsubnet(step2, step3, device=DEVICE)

    # 4) VoiceFixer (вокал)
    from .voicefixer_wrapper import VoiceFixer
    step4 = tmp / "04_voicefixed.wav"
    vf = VoiceFixer(device=DEVICE, mode=1)
    vf.process(step3, step4)

    # 5) Удаление винилового шума
    from .noise_reduction import reduce_vinyl_noise
    step5 = tmp / "05_nr.wav"
    reduce_vinyl_noise(step4, step5, stationary=True, prop_decrease=0.2)

    # 6) Мастеринг (LUFS нормализация + лимитинг)
    from .mastering import spectral_master_full
    final = tmp / "06_master.wav"
    spectral_master_full(step5, final, target_lufs=-14.0, peak_limit=-0.3)

    # 7) ASR + жанровая классификация (не роняем пайплайн при ошибке)
    text, category = "", "unknown"
    try:
        from .stt_whisper import transcribe
        from .classifier import clf
        text = transcribe(final)
        category = clf.predict(text)
    except Exception as e:
        logging.warning("ASR/genre tagging failed: %s", e)

    return {
        "restored_path": final.name,
        "text": text,
        "category": category,
    }