import logging
import uuid
from pathlib import Path

import whisper
import torch
from demucs.pretrained import get_model
from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import FileResponse

from config import AUDIO_TEMP_DIR, DEVICE
from pipeline.voicefixer_wrapper import VoiceFixer  # обёртка над VoiceFixer с подавлением sys.exit
logging.basicConfig(level=logging.INFO, format="%(asctime)s %(levelname)s %(message)s")
app = FastAPI(title="War Song ML Service")
from fastapi.middleware.cors import CORSMiddleware

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],            # или список конкретных origin-ов
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)
# Глобальные переменные для моделей
_whisper_model: whisper.Whisper | None = None
_vocfixer: VoiceFixer | None = None

@app.on_event("startup")
def preload_models():
    """
    На старте сервера сразу загружаем все тяжёлые модели в память,
    чтобы запросы не тянули инициализацию.
    """
    global _whisper_model, _vocfixer

    logging.info("Загружаем Whisper small...")
    _whisper_model = whisper.load_model("small", download_root="/root/.cache/whisper")

    logging.info("Загружаем Demucs htdemucs...")
    get_model("htdemucs").cpu().eval()

    logging.info("Инициализируем VoiceFixer...")
    _vocfixer = VoiceFixer(device=DEVICE)

    logging.info("Все модели загружены и готовы к работе.")

@app.post("/process")
def full_pipeline(file: UploadFile = File(...)):
    """
    Единая точка входа для обработки .wav-файла:
      - сохраняем файл во временную папку
      - вызываем pipeline.process()
      - возвращаем JSON с результатами
    """
    # Проверяем расширение
    if not file.filename.lower().endswith(".wav"):
        raise HTTPException(status_code=400, detail="Поддерживаются только .wav файлы")

    # Готовим временную директорию
    tmp_dir = AUDIO_TEMP_DIR / uuid.uuid4().hex
    tmp_dir.mkdir(parents=True, exist_ok=True)

    in_path = tmp_dir / file.filename
    # Сохраняем входящий файл
    data = file.file.read()
    in_path.write_bytes(data)

    # Запускаем весь пайплайн
    from pipeline.pipeline import process

    try:
        result = process(in_path)
    except SystemExit as se:
        # Ловим любой sys.exit в глубине, чтобы не убить Uvicorn
        logging.exception(f"Подавлен SystemExit({se.code}) в процессе обработки")
        raise HTTPException(status_code=500, detail="Внутренняя ошибка ML-сервиса")
    except Exception:
        logging.exception("Пайплайн обработки упал с ошибкой")
        raise HTTPException(status_code=500, detail="Внутренняя ошибка ML-сервиса")

    return result

@app.get("/download/{fname}")
def download(fname: str):
    """
    Возвращает готовый файл пользователю по его имени.
    """
    path = (AUDIO_TEMP_DIR / fname).resolve()
    if not path.exists():
        raise HTTPException(status_code=404, detail="Файл не найден")
    return FileResponse(path)
