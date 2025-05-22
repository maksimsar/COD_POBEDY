"""
pipeline.mastering
==================

• spectral_master_full()  – LUFS normalisation + brick-wall clipping
• spectral_master()       – legacy alias (calls *_full)

Both return Path to the written WAV.
"""

from pathlib import Path
import numpy as np
import pyloudnorm as pyln

# thin wrappers around your helpers
from utils.audio import load_wav, save_wav


def spectral_master_full(
    wav_in: Path,
    wav_out: Path,
    *,
    target_lufs: float = -14.0,   # Spotify / YouTube music guideline
    peak_limit: float = -0.3      # final hard limit (dBFS)
) -> Path:
    """
    1) Measure integrated loudness → match `target_lufs`
    2) Hard-clip everything above `peak_limit` dBFS
    """
    y, sr = load_wav(wav_in)

    # --- LUFS normalisation ---
    meter    = pyln.Meter(sr)
    loudness = meter.integrated_loudness(y)
    gain_db  = target_lufs - loudness
    y        = y * (10 ** (gain_db / 20))

    # --- brick-wall limiter ---
    peak_amp = 10 ** (peak_limit / 20)
    y        = np.clip(y, -peak_amp, peak_amp)

    save_wav(y, sr, wav_out)
    return wav_out


# ---------- backwards-compat alias ----------
def spectral_master(*args, **kwargs):
    """
    Deprecated name kept so that old imports
        from pipeline.mastering import spectral_master
    keep working transparently.
    """
    return spectral_master_full(*args, **kwargs)
