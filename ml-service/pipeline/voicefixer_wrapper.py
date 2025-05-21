# pipeline/voicefixer_wrapper.py

import logging
from pathlib import Path
import voicefixer  # pip install git+https://github.com/haoheliu/voicefixer.git

class VoiceFixer:
    """
    Обёртка над Python API VoiceFixer:
      - инициализируем один раз
      - на каждом .process() вызываем .restore()
    """
    def __init__(self, device: str = "cpu", mode: int | str = 0, silent: bool = True):
        """
        :param device: "cpu" или "cuda"
        :param mode: 0,1,2 или "all"
        :param silent: если False — будут логи из voicefixer
        """
        self.cuda = (device == "cuda")
        self.mode = mode
        self.silent = silent
        logging.info(f"[VoiceFixer] Init: cuda={self.cuda}, mode={self.mode}")
        # создаём экземпляр модели once
        self._vf = voicefixer.VoiceFixer()

    def process(self, input_path: Path, output_path: Path) -> None:
        inp = Path(input_path)
        out = Path(output_path)
        if not inp.exists():
            raise FileNotFoundError(f"VoiceFixer input file not found: {inp}")

        logging.info(f"[VoiceFixer] restore: in={inp}, out={out}, cuda={self.cuda}, mode={self.mode}")
        try:
            # вызываем restore без параметра silent
            self._vf.restore(
                input=str(inp),
                output=str(out),
                cuda=self.cuda,
                mode=self.mode
            )
        except Exception:
            logging.exception("[VoiceFixer] Python API failed")
            raise RuntimeError("VoiceFixer не удалось запустить") from None

