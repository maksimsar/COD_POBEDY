"""
Поп-фильтр на базе librosa.declick + настраиваемый fallback.
"""
from pathlib import Path
import subprocess
import numpy as np
import scipy.signal
import soundfile as sf
import librosa

def _fallback(y: np.ndarray, sr: int) -> np.ndarray:
    # чуть более толстая медианная фильтрация + строгий порог
    frame = int(0.002 * sr)  # 2 ms
    kernel = frame if frame % 2 == 1 else frame + 1
    med = scipy.signal.medfilt(y, kernel_size=kernel)
    delta = y - med
    # порог — 6σ, вместо процентилей
    sigma = np.std(delta)
    mask = np.abs(delta) > 6 * sigma
    y_clean = y.copy()
    y_clean[mask] = med[mask]
    return y_clean

def remove_clicks(wav_in: Path, wav_out: Path) -> Path:
    try:
        # если есть внешний crcd
        subprocess.run(
            ["crcd", wav_in.as_posix(), wav_out.as_posix()],
            check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
        )
    except (FileNotFoundError, subprocess.CalledProcessError):
        # основной декликер
        y, sr = librosa.load(wav_in.as_posix(), sr=None)
        try:
            # top_db уменьшает агрессивность удаления; fill_value="interpolate" заполняет щели
            y = librosa.effects.declick(y, top_db=15, fill_value="interpolate")
        except Exception:
            # если librosa.declick выдал ошибку — fallback
            y = _fallback(y, sr)
        # слегка прогладим края (коррекция граничных щелчков)
        y = librosa.effects.preemphasis(y, coef=0.90)
        sf.write(wav_out.as_posix(), y, sr)
    return wav_out
