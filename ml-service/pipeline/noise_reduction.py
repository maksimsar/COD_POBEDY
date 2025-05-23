# pipeline/noise_reduction.py
import numpy as np
from scipy.io import wavfile
import noisereduce as nr
from pathlib import Path

def reduce_vinyl_noise(
    wav_in: Path | str,
    wav_out: Path | str,
    noise_sample: Path | None = None,
    stationary: bool = True,
    prop_decrease: float = 0.15,
    n_std_thresh_stationary: float = 2.0,
    time_mask_smooth_ms: int = 80,
    freq_mask_smooth_hz: int = 600,
    blend_alpha: float = 0.15,
    n_fft: int = 1024,
    hop_length: int = 256,
) -> Path:
    """
    Тонкое удаление фонового винилового шума.
    Смешивание: 85% оригинала + 15% очистки.
    """
    wav_in = Path(wav_in)
    wav_out = Path(wav_out)

    sr, data = wavfile.read(str(wav_in))
    orig_dtype = data.dtype
    y = data.astype(np.float32)

    # профиль шума
    y_noise = None
    if noise_sample is not None:
        _, y_noise = wavfile.read(str(noise_sample))
        y_noise = y_noise.astype(np.float32)

    # спектральное шумоподавление
    reduced = nr.reduce_noise(
        y=y,
        sr=sr,
        y_noise=y_noise,
        stationary=stationary,
        prop_decrease=prop_decrease,
        n_std_thresh_stationary=n_std_thresh_stationary,
        time_mask_smooth_ms=time_mask_smooth_ms,
        freq_mask_smooth_hz=freq_mask_smooth_hz,
        n_fft=n_fft,
        hop_length=hop_length,
    )

    # мягкое смешивание: почти оригинал + чуть-чуть очищенного
    blended = (1 - blend_alpha) * y + blend_alpha * reduced

    # приводим обратно к исходному формату
    out = np.clip(blended, np.iinfo(orig_dtype).min, np.iinfo(orig_dtype).max)
    wavfile.write(str(wav_out), sr, out.astype(orig_dtype))

    return wav_out
