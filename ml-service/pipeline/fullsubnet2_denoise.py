# pipeline/fullsubnet2_denoise.py
import sys
import logging
from pathlib import Path
import torch

def denoise_fullsubnet(
    wav_in: Path,
    wav_out: Path,
    device: str = "cpu",
) -> None:
    repo_dir = Path(__file__).parent / "fullsubnet_plus"
    out_dir = wav_out.parent
    out_dir.mkdir(parents=True, exist_ok=True)

    logging.info(f"🔈 FullSubNet2: загрузка модели из {repo_dir}, device={device}")
    # Передаём input_wav и output_dir прямо в hub.load
    pipeline = torch.hub.load(
        repo_or_dir=str(repo_dir),
        model="FullSubNet2",
        source="local",
        device=device,
        input_wav=str(wav_in),
        output_dir=str(out_dir),
    )

    # Подменяем sys.exit, чтобы внешние exit() не убили Uvicorn
    orig_exit = sys.exit
    sys.exit = lambda code=0: logging.warning(f"Ignored sys.exit({code}) from FullSubNet2")

    try:
        logging.info("🚀 FullSubNet2: запуск инференса…")
        # Если у объекта есть явный run – вызываем его
        if hasattr(pipeline, "run"):
            pipeline.run()
        else:
            # иначе – вызываем сам объект, он внутри hubconf запустит инференс
            pipeline()
    except SystemExit as se:
        logging.info(f"Caught SystemExit({se.code}) from FullSubNet2, продолжаем")
    except Exception:
        logging.exception("Ошибка инференса FullSubNet2")
        raise RuntimeError("FullSubNet2 не удалось прогнать") from None
    finally:
        sys.exit = orig_exit

    # Ищем результат
    candidate = out_dir / wav_in.name
    if candidate.exists():
        candidate.replace(wav_out)
        logging.info(f"✅ FullSubNet2: результат записан в {wav_out}")
        return

    # Fallback: любой другой .wav
    wavs = [p for p in out_dir.glob("*.wav") if p.name != wav_in.name]
    if not wavs:
        raise FileNotFoundError(f"Не найден ни один .wav в {out_dir}")
    wavs[0].replace(wav_out)
    logging.info(f"✅ FullSubNet2 (fallback): результат записан в {wav_out}")