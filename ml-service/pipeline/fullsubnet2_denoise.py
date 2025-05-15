from pathlib import Path
import torch

def denoise_fullsubnet(wav_in: Path, wav_out: Path, device: str = "cpu"):
    """
    Используем FullSubNet2 через точку входа torch.hub.load:
      - передаём ему конкретный WAV-файл (input_wav)
      - и папку, куда он сохранит обработанный результат (output_dir)
      - затем вызываем pipeline() — он сам запишет .wav в output_dir.
      - в конце переименовываем полученный файл в wav_out.
    """
    repo_dir = Path(__file__).parent / "FullSubNet-plus"

    # Подготовим папку для результата
    output_dir = wav_out.parent
    output_dir.mkdir(parents=True, exist_ok=True)

    # Подгружаем новый pipeline-инстанс
    pipeline = torch.hub.load(
        repo_or_dir=str(repo_dir),
        model="FullSubNet2",
        source="local",
        device=device,
        input_wav=str(wav_in),
        output_dir=str(output_dir),
    )

    # Запускаем инференс (сохранит файл в output_dir/<basename>.wav)
    pipeline()

    # Ожидаемый путь обработанного файла
    result = output_dir / wav_in.name
    if not result.exists():
        raise FileNotFoundError(f"Не нашёл результат по {result}")
    # Переносим в точный путь wav_out
    result.rename(wav_out)
