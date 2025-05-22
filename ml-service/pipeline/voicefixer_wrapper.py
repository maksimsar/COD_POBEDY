import logging
from pathlib import Path
import numpy as np
import soundfile as sf
import voicefixer
from scipy.signal import resample


def _soft_mix(original: Path, restored: Path, output: Path, alpha: float = 0.7) -> None:
    """
    Soft mix: смешивает original и restored в пропорции (1-alpha):alpha,
    выравнивает длину и число каналов, при необходимости ресэмплит.
    """
    # Чтение аудио
    y0, sr0 = sf.read(str(original))
    y1, sr1 = sf.read(str(restored))
    # Убедимся, что частоты одинаковы
    if sr0 != sr1:
        logging.warning(f"Sampling rates differ ({sr1} vs {sr0}), ресэмплим restored")
        if y1.ndim == 1:
            y1 = resample(y1, int(len(y1) * sr0 / sr1))
        else:
            # ресэмплим каждый канал
            channels = []
            for i in range(y1.shape[1]):
                channels.append(resample(y1[:, i], int(len(y1) * sr0 / sr1)))
            y1 = np.stack(channels, axis=1)
    # Уравниваем число каналов
    if y0.ndim == 2 and y1.ndim == 1:
        y1 = np.stack([y1, y1], axis=1)
    elif y0.ndim == 1 and y1.ndim == 2:
        y1 = y1.mean(axis=1)
    # Уравниваем длину
    minlen = min(y0.shape[0], y1.shape[0])
    y0 = y0[:minlen]
    y1 = y1[:minlen]
    # Soft mix
    y = (1 - alpha) * y0 + alpha * y1
    # Запись результата
    sf.write(str(output), y, sr0)


class VoiceFixer:
    """
    Обёртка над Python API VoiceFixer с мягким микшированием.
    """
    def __init__(self, device: str = "cpu", mode: int | str = 0, silent: bool = True):
        self.cuda = device.startswith("cuda")
        self.mode = mode
        self.silent = silent
        logging.info(f"[VoiceFixer] Init: cuda={self.cuda}, mode={self.mode}")
        # создаём экземпляр модели один раз
        self._vf = voicefixer.VoiceFixer()

    def process(self, input_path: Path, output_path: Path, alpha: float = 0.1) -> None:
        """
        1) Запускает VoiceFixer.restore() → temp_out
        2) Мягко смешивает input_path и temp_out → output_path
        """
        inp = Path(input_path)
        out = Path(output_path)
        if not inp.exists():
            raise FileNotFoundError(f"VoiceFixer input file not found: {inp}")

        # Промежуточный файл
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

        # Soft mix оригинала и восстановления
        try:
            _soft_mix(inp, temp_out, out, alpha=alpha)
        except Exception:
            logging.exception("[VoiceFixer] Soft mix failed, копируем raw выход")
            temp_out.replace(out)
        finally:
            # Удаляем временный файл
            if temp_out.exists():
                temp_out.unlink()
