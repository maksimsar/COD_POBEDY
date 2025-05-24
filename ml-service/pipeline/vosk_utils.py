import wave
import json
from pathlib import Path
from vosk import Model, KaldiRecognizer

def align_lyrics(
    wav_path: Path | str,
    model: Model
) -> list[dict]:
    """
    Возвращает список слов с таймкодами из WAV:
      [{'word': str, 'start': float, 'end': float}, …]
    """
    wf = wave.open(str(wav_path), "rb")
    rec = KaldiRecognizer(model, wf.getframerate())
    rec.SetWords(True)

    words: list[dict] = []
    while True:
        chunk = wf.readframes(4000)
        if not chunk:
            break
        if rec.AcceptWaveform(chunk):
            res = json.loads(rec.Result())
            words.extend(res.get("result", []))

    final = json.loads(rec.FinalResult())
    words.extend(final.get("result", []))
    wf.close()
    return words