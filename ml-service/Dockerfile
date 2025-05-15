# 1) Базовый образ с Python 3.10; там есть wheel-ы deepfilterlib для cp310
FROM python:3.10-slim

# 2) Устанавливаем системные зависимости для аудио и сборки
RUN apt-get update \
 && apt-get install -y --no-install-recommends \
      libsndfile1 libsndfile1-dev \
      build-essential \
 && rm -rf /var/lib/apt/lists/*

# 3) Рабочая папка
WORKDIR /app

# 4) Копируем ваши файлы
COPY . /app

# 5) Обновляем pip и ставим зависимости
RUN pip install --upgrade pip setuptools wheel \
 && pip install deepfilternet==0.5.6 \
 && pip install -r requirements.txt

# 6) Открываем порт
EXPOSE 8000

# 7) Запускаем uvicorn
CMD ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "8000"]
