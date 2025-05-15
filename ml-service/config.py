from pathlib import Path

BASE_DIR       = Path(__file__).parent
MODEL_DIR      = BASE_DIR / "models"
AUDIO_TEMP_DIR = BASE_DIR / "tmp"
AUDIO_TEMP_DIR.mkdir(exist_ok=True)

DEVICE = "cuda"  # или "cpu"
