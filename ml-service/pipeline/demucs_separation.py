from pathlib import Path
import subprocess, shutil, tempfile, glob

def separate_vocals(wav_in: Path, wav_out: Path) -> Path:
    with tempfile.TemporaryDirectory() as tmp:
        # запускаем CLI
        subprocess.run(
            [
                "python", "-m", "demucs.separate",
                "--two-stems", "vocals",
                "-n", "htdemucs",          # модель
                "-o", tmp,
                wav_in.as_posix(),
            ],
            check=True,
        )

        # ищем vocals.wav где-угодно в выходной папке
        matches = list(Path(tmp).rglob("vocals.wav"))
        if not matches:
            raise RuntimeError("Demucs did not create vocals.wav")
        shutil.move(matches[0], wav_out)

    return wav_out
