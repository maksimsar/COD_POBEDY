from fastapi import FastAPI, UploadFile
app = FastAPI()

@app.post("/denoise")
async def denoise(file: UploadFile):
    return {"message": "здесь будет логика очистки"}

@app.post("/transcribe")
async def transcribe(file: UploadFile):
    return {"message": "здесь будет логика транскрипции"}
