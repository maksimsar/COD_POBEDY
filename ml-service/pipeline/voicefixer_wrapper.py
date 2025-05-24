import logging
from pathlib import Path
import numpy as np
import soundfile as sf
import voicefixer
from scipy.signal import resample


def _soft_mix(original: Path, restored: Path, output: Path, alpha: float = 0.7) -> None:
    y0, sr0 = sf.read(str(original))
    y1, sr1 = sf.read(str(restored))

    # Приводим restored к нужной частоте
    if sr0 != sr1:
        logging.warning(f"Sampling rates differ ({sr1} vs {sr0}), ресэмплим restored")
        if y1.ndim == 1:
            y1 = resample(y1, int(len(y1) * sr0 / sr1))
        else:
            y1 = np.stack([resample(y1[:, i], int(len(y1) * sr0 / sr1)) for i in range(y1.shape[1])], axis=1)
        sr1 = sr0  # теперь частоты совпадают

    # Выравниваем число каналов
    if y0.ndim == 2 and y1.ndim == 1:
        y1 = np.stack([y1, y1], axis=1)
    elif y0.ndim == 1 and y1.ndim == 2:
        y1 = y1.mean(axis=1)

    # Обрезаем до общей длины
    minlen = min(y0.shape[0], y1.shape[0])
    y0 = y0[:minlen]
    y1 = y1[:minlen]

    y = (1 - alpha) * y0 + alpha * y1
    sf.write(str(output), y, sr0)


class VoiceFixer:
    def __init__(self, device: str = "cpu", mode: int | str = 1, silent: bool = True):
        self.cuda = device.startswith("cuda")
        self.mode = mode
        self.silent = silent
        logging.info(f"[VoiceFixer] Init: cuda={self.cuda}, mode={self.mode}")
        self._vf = voicefixer.VoiceFixer()

    def process(self, input_path: Path, output_path: Path, alpha: float = 0.1, target_sr: int = 44100) -> None:
        inp = Path(input_path)
        out = Path(output_path)
        if not inp.exists():
            raise FileNotFoundError(f"VoiceFixer input file not found: {inp}")

        temp_out = out.with_name(out.stem + "_raw" + out.suffix)

        logging.info(f"[VoiceFixer] restore: in={inp}, temp_out={temp_out}, cuda={self.cuda}, mode={self.mode}")
        try:
            self._vf.restore(
                input=str(inp),
                output=str(temp_out),
                cuda=self.cuda,
                mode=self.mode
            )
        except Exception:
            logging.exception("[VoiceFixer] Python API failed")
            raise RuntimeError("VoiceFixer не удалось запустить") from None

        # Приведение к нужному sr при необходимости
        try:
            sr_restored = sf.info(temp_out).samplerate
            if sr_restored != target_sr:
                y_restored, _ = sf.read(temp_out)
                if y_restored.ndim == 1:
                    y_resampled = resample(y_restored, int(len(y_restored) * target_sr / sr_restored))
                else:
                    y_resampled = np.stack([
                        resample(y_restored[:, i], int(len(y_restored) * target_sr / sr_restored))
                        for i in range(y_restored.shape[1])
                    ], axis=1)
                sf.write(temp_out, y_resampled, target_sr)
        except Exception:
            logging.exception("[VoiceFixer] Resample failed")

        # Soft mix
        try:
            _soft_mix(inp, temp_out, out, alpha=alpha)
        except Exception:
            logging.exception("[VoiceFixer] Soft mix failed, копируем raw выход")
            temp_out.replace(out)
        finally:
            if temp_out.exists():
                temp_out.unlink()
