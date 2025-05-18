# pipeline/deepfilternet_denoise.py

import shutil
from pathlib import Path

def denoise_df(wav_in: Path, wav_out: Path):
    """
    Заглушка вместо DeepFilterNet:
    просто копируем входной WAV в выходной.
    """
    shutil.copy(wav_in, wav_out)

