FROM python:3.10-slim

# 1) Системные пакеты
RUN apt-get update \
 && apt-get install -y --no-install-recommends \
      git ffmpeg libsndfile1 build-essential curl ca-certificates wget \
 && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# 2) Pip без кеша
ENV PIP_NO_CACHE_DIR=1 \
    PIP_DEFAULT_TIMEOUT=120 \
    PIP_RETRIES=10

# 3) Базовые инструменты
RUN pip install --upgrade pip setuptools wheel \
    pyloudnorm==0.1.1 rich tqdm

# 4) requirements.txt
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# 5) PyTorch + torchaudio CPU
ARG TORCH_VER=2.6.0
ARG PYTORCH_URL=https://download.pytorch.org/whl/cpu
RUN pip install --no-cache-dir \
    torch==${TORCH_VER}+cpu \
    torchaudio==${TORCH_VER} \
    --extra-index-url ${PYTORCH_URL}

# 6) VoiceFixer и аудио-библиотеки
RUN pip install --no-cache-dir \
    git+https://github.com/haoheliu/voicefixer.git \
    matplotlib==3.8.4 \
    contourpy>=1.0.1 fonttools>=4.22.0 kiwisolver>=1.4.4 cycler>=0.11.0 \
    librosa==0.11.0 torchlibrosa==0.1.0 \
    GitPython progressbar2 \
    python-multipart toml pillow pyparsing

# 7) FastAPI + Uvicorn
RUN pip install --no-cache-dir fastapi==0.110.2 uvicorn==0.29.0

# 8) Transformers, Demucs, Whisper
RUN pip install --no-cache-dir \
    transformers==4.51.3 sentencepiece accelerate==1.7.0 \
    demucs==4.0.0 openai-whisper==20240930

# 8.1) Предзагружаем small-модель Whisper
RUN python - <<EOF
import whisper
whisper.load_model("small", download_root="/root/.cache/whisper") #large-v2
EOF

# 9) Весы VoiceFixer и Demucs
COPY weights/vf.ckpt /root/.cache/voicefixer/analysis_module/checkpoints/vf.ckpt
COPY weights/vocoder.pt /root/.cache/voicefixer/synthesis_module/44100/model.ckpt-1490000_trimed.pt
ENV TORCH_HOME=/opt/torch_cache
COPY weights/htdemucs.th ${TORCH_HOME}/hub/checkpoints/955717e8-8726e21a.th

# 10) DDD-Declipper
RUN git clone https://github.com/stet-stet/DDD.git /opt/ddd \
 && grep -vE '^(torch|torchaudio)' /opt/ddd/requirements.txt > /opt/ddd/req_clean.txt \
 && pip install --no-cache-dir -r /opt/ddd/req_clean.txt

# 11) Копируем приложение
COPY config.py app.py /app/
COPY pipeline/ /app/pipeline/
COPY utils/    /app/utils/

# 12) PYTHONPATH, чтобы ваш hubconf и DDD-шаблон находились первыми
ENV PYTHONPATH="/app/pipeline/fullsubnet_plus:/opt/ddd:${PYTHONPATH}"

# 13) FullSubNet2: модель
RUN mkdir -p pipeline/fullsubnet_plus/pretrained \
 && wget -q -O pipeline/fullsubnet_plus/pretrained/best_model.tar \
     "https://docs.google.com/uc?export=download&confirm=t&id=1UJSt1G0P_aXry-u79LLU_l9tCnNa2u7C"

# 14) Запуск с увеличенным keep-alive
EXPOSE 8000

# 7) Запускаем uvicorn
CMD ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "8000"]
