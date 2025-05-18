from pathlib import Path
import uuid
from .deepfilternet_denoise import denoise_df
from config import AUDIO_TEMP_DIR, DEVICE
from . import (remove_clicks, separate_vocals, denoise_df,
               denoise_fullsubnet, spectral_master, transcribe)
#from classifier.model import BertClassifier

#clf = BertClassifier(device=DEVICE)

def process(audio_in: Path, do_master=True):
    tmp_base = AUDIO_TEMP_DIR / uuid.uuid4().hex
    tmp_base.mkdir(exist_ok=True)

    step1 = tmp_base / "01_clicks.wav"
    remove_clicks(audio_in, step1)

    step2 = tmp_base / "02_vocals.wav"
    separate_vocals(step1, step2)

    step3 = tmp_base / "03_denoise_df.wav"
    denoise_df(step2, step3)

    step4 = tmp_base / "04_denoise_fs.wav"
    denoise_fullsubnet(step3, step4, device=DEVICE)

    step5 = tmp_base / "05_master.wav"
    final = spectral_master(step4, step5) if do_master else step4

    text      = transcribe(final)
    category  = clf.predict(text)

    return {
        "restored_path": final,
        "text":          text,
        "category":      "unknown",
    }
