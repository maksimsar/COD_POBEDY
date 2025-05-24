from pathlib import Path
import soundfile as sf

def save_wav(y, sr, path: Path):
    path.parent.mkdir(exist_ok=True, parents=True)
    sf.write(path.as_posix(), y, sr)

def load_wav(path: Path):
    y, sr = sf.read(path.as_posix())
    if y.ndim > 1:                     # stereo → mono
        y = y.mean(axis=1)
    return y, sr
