"""
Поп-фильтр на базе Pop2k/CRCD — если установлен одноимённый CLI-бинари.
Fallback : простая статистическая импульсная фильтрация.
"""
from pathlib import Path
import subprocess
import numpy as np
import scipy.signal
from utils.audio import load_wav, save_wav

def _fallback(y, sr):
    frame = int(0.001 * sr)                     # 1 мс
    med   = scipy.signal.medfilt(y, kernel_size=frame|1)
    delta = y - med
    mask  = np.abs(delta) > 0.75 * np.percentile(np.abs(delta), 95)
    y[mask] = med[mask]
    return y

def remove_clicks(wav_in: Path, wav_out: Path) -> Path:
    try:                                         # Pop2k/CRCD установлен?
        subprocess.run(
            ["crcd", wav_in.as_posix(), wav_out.as_posix()],
            check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
        )
    except (FileNotFoundError, subprocess.CalledProcessError):
        y, sr = load_wav(wav_in)
        y = _fallback(y, sr)
        save_wav(y, sr, wav_out)
    return wav_out
