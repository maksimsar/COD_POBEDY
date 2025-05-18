from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import FileResponse
from pathlib import Path
from pipeline.pipeline import process
from config import AUDIO_TEMP_DIR

app = FastAPI(title="War Song ML Service")

@app.post("/process")
async def full_pipeline(file: UploadFile = File(...)):
    if not file.filename.endswith(".wav"):
        raise HTTPException(400, "Только .wav")

    path_in = AUDIO_TEMP_DIR / file.filename
    with open(path_in, "wb") as f:
        f.write(await file.read())

    out = process(path_in)
    return {
        "text":     out["text"],
        "category": out["category"],
    }

@app.get("/download/{name}")
def download(name: str):
    path = (AUDIO_TEMP_DIR / name).resolve()
    if not path.exists():
        raise HTTPException(404)
    return FileResponse(path)
