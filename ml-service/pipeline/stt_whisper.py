# pipeline/stt_whisper.py
import os
import whisper
import logging
from pathlib import Path

# Кеш модели
_WHISPER_MODEL = None

def _load_model(device: str = "cpu"):
    global _WHISPER_MODEL
    if _WHISPER_MODEL is None:
        # Разворачиваем ~ в абсолютный путь
        cache_dir = os.path.expanduser("~/.cache/whisper")
        os.makedirs(cache_dir, exist_ok=True)
        try:
            # small ≈ 245 MB, faster загрузка и инференс
            _WHISPER_MODEL = whisper.load_model("small", device=device, download_root=cache_dir)
        except Exception:
            logging.exception("Не смогли загрузить Whisper-модель")
            raise
    return _WHISPER_MODEL

def transcribe(wav_path: Path, language: str = "ru", device: str = "cpu") -> str:
    """
    Возвращает транскрибированный текст для данного .wav-файла.
    """
    try:
        model = _load_model(device=device)
        result = model.transcribe(str(wav_path), language=language, fp16=False)
        return result.get("text", "").strip()
    except Exception:
        logging.exception("Ошибка трансформации audio→text")
        # В случае провала ASR не падаем весь пайплайн, просто возвращаем пустую строку
        return ""
