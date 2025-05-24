"""
pipeline.mastering
==================

• spectral_master_full()  – LUFS нормализация + brick-wall клиппинг + EQ-срез высоких частот + мультибендовый шумогейт
• spectral_master()       – legacy alias (calls *_full)

Both return Path to the written WAV.
"""

from pathlib import Path
import numpy as np
import pyloudnorm as pyln
from scipy.signal import butter, sosfiltfilt

# thin wrappers around your helpers
from utils.audio import load_wav, save_wav


def high_cut_filter(
    y: np.ndarray,
    sr: int,
    cutoff: float = 18000.0,
    order: int = 4
) -> np.ndarray:
    """
    Накладывает низкочастотный фильтр Butterworth, срезая всё выше cutoff (Гц).
    """
    nyq = sr / 2.0
    normal_cutoff = cutoff / nyq
    sos = butter(order, normal_cutoff, btype='low', output='sos')
    return sosfiltfilt(sos, y)


def multiband_noise_gate(
    y: np.ndarray,
    sr: int,
    split_freq: float = 2000.0,
    threshold_db: float = -50.0,
    reduction: float = 0.1,
    order: int = 4
) -> np.ndarray:
    """
    Применяет шумогейт только к высокочастотной части выше split_freq.
    Низкие частоты остаются без изменений, а в высоких, если амплитуда
    ниже threshold_db, сигнал ослабляется на коэффициент reduction.
    """
    nyq = sr / 2.0

    # низкочастотная часть
    sos_low = butter(order, split_freq / nyq, btype='low', output='sos')
    y_low = sosfiltfilt(sos_low, y)

    # высокочастотная часть
    sos_high = butter(order, split_freq / nyq, btype='high', output='sos')
    y_high = sosfiltfilt(sos_high, y)

    # шумогейт на высоких частотах
    thresh_amp = 10 ** (threshold_db / 20.0)
    mask = np.abs(y_high) < thresh_amp
    y_high[mask] *= reduction

    return y_low + y_high


def spectral_master_full(
    wav_in: Path,
    wav_out: Path,
    *,
    target_lufs: float = -14.0,       # Spotify / YouTube guideline
    peak_limit: float = -0.3,         # final hard limit (dBFS)
    eq_cutoff: float = 18000.0,       # срез высоких частот
    gate_split_freq: float = 2000.0,  # граница для шумогейта
    gate_threshold_db: float = -50.0, # порог шумогейта
    gate_reduction: float = 0.1       # коэффициент ослабления
) -> Path:
    """
    1) LUFS-нормализация до target_lufs
    2) Brick-wall лимитер для пиков > peak_limit
    3) EQ: срез высоких частот > eq_cutoff
    4) Мультибендовый шумогейт на высоких частотах
    """
    y, sr = load_wav(wav_in)

    # --- LUFS нормализация ---
    meter    = pyln.Meter(sr)
    loudness = meter.integrated_loudness(y)
    gain_db  = target_lufs - loudness
    y        = y * (10 ** (gain_db / 20.0))

    # --- Brick-wall лимитер ---
    peak_amp = 10 ** (peak_limit / 20.0)
    y        = np.clip(y, -peak_amp, peak_amp)

    # --- EQ: срез высоких частот ---
    y = high_cut_filter(y, sr, cutoff=eq_cutoff)

    # --- Мультибендовый шумогейт ---
    y = multiband_noise_gate(
        y,
        sr,
        split_freq=gate_split_freq,
        threshold_db=gate_threshold_db,
        reduction=gate_reduction
    )

    save_wav(y, sr, wav_out)
    return wav_out


# ---------- backwards-compat alias ----------
def spectral_master(*args, **kwargs):
    """
    Deprecated name kept so that:
        from pipeline.mastering import spectral_master
    continues to work.
    """
    return spectral_master_full(*args, **kwargs)
