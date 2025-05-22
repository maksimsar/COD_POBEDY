from pathlib import Path
import subprocess
import numpy as np
import scipy.signal
import soundfile as sf
import librosa

def _fallback(y: np.ndarray, sr: int) -> np.ndarray:
    """
    Мягкий fallback: медианная фильтрация с более высоким порогом и сглаживающим микшированием.
    """
    # 5 ms кадр для медианной фильтрации
    frame = int(0.005 * sr)
    kernel = frame if frame % 2 == 1 else frame + 1
    med = scipy.signal.medfilt(y, kernel_size=kernel)
    delta = y - med
    # более строгий порог — 8σ
    sigma = np.std(delta)
    threshold = max(8 * sigma, 0.001)  # не ниже 0.1%
    mask = np.abs(delta) > threshold
    y_clean = y.copy()
    y_clean[mask] = med[mask]
    # мягкое смешивание: 90% оригинала + 10% очищенного
    y_blend = 0.9 * y + 0.1 * y_clean
    return y_blend

def remove_clicks(wav_in: Path, wav_out: Path) -> Path:
    """
    Поп-фильтр на базе librosa.declick + настраиваемый soft-fallback.
    """
    try:
        # если есть внешний crcd
        subprocess.run(
            ["crcd", wav_in.as_posix(), wav_out.as_posix()],
            check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
        )
    except (FileNotFoundError, subprocess.CalledProcessError):
        y, sr = librosa.load(wav_in.as_posix(), sr=None)
        y_clean = None
        try:
            # снижаем агрессивность: top_db уменьшен до 12
            # используем более длинные фреймы
            y_clean = librosa.effects.declick(
                y,
                top_db=12,
                frame_length=int(0.005 * sr),
                hop_length=int(0.0025 * sr),
                fill_value="interpolate"
            )
        except Exception:
            # fallback
            y_clean = _fallback(y, sr)

        # если декликер ничего не сделал (или слишком агрессивно), смешиваем с оригиналом
        if y_clean is not None and y_clean.shape == y.shape:
            # 85% оригинала + 15% очищенного
            y = 0.85 * y + 0.15 * y_clean
        # минимальная предэмфаза для смягчения граничных артефактов
        y = librosa.effects.preemphasis(y, coef=0.85)
        sf.write(wav_out.as_posix(), y, sr)
    return wav_out
