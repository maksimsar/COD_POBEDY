from pathlib import Path
import pyloudnorm as pyln
from utils.audio import load_wav, save_wav

def spectral_master(wav_in: Path, wav_out: Path, target_lufs=-14.0) -> Path:
    y, sr = load_wav(wav_in)
    meter   = pyln.Meter(sr)
    loudness = meter.integrated_loudness(y)
    gain_db  = target_lufs - loudness
    y = y * (10 ** (gain_db / 20))
    save_wav(y, sr, wav_out)
    return wav_out
