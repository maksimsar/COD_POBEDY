#!/usr/bin/env python
# coding: utf-8

import argparse
from pathlib import Path

import pandas as pd
from datasets import load_dataset
from transformers import (
    AutoTokenizer,
    AutoModelForSequenceClassification,
    Trainer,
    TrainingArguments,
)

def main():
    parser = argparse.ArgumentParser(description="Train song-text classifier")
    parser.add_argument(
        "--csv_path",
        type=str,
        default="data/song_labels.csv",
        help="Путь к CSV с колонками text,label",
    )
    parser.add_argument(
        "--pretrained_model",
        type=str,
        default="DeepPavlov/rubert-base-cased",
        help="Имя предобученной модели в HuggingFace",
    )
    parser.add_argument(
        "--output_dir",
        type=str,
        default="models/bert-song-classifier",
        help="Куда сохранять обученную модель и токенизатор",
    )
    args = parser.parse_args()

    # Читаем CSV и строим mapping для меток
    df = pd.read_csv(args.csv_path)
    labels = df["label"].unique().tolist()
    label2id = {label: idx for idx, label in enumerate(labels)}
    id2label = {idx: label for label, idx in label2id.items()}

    # Загружаем датасет и токенизатор
    dataset = load_dataset("csv", data_files={"train": args.csv_path})["train"]
    tokenizer = AutoTokenizer.from_pretrained(args.pretrained_model)

    # Функция препроцессинга: токенизация + перевод label → id
    def preprocess(batch):
        tokens = tokenizer(
            batch["text"],
            truncation=True,
            padding="max_length",
            max_length=128,
        )
        # создаём численные метки под ключом "labels"
        tokens["labels"] = [label2id[l] for l in batch["label"]]
        return tokens

    # Мапим и сразу удаляем старые колонки text,label
    tokenized = dataset.map(
        preprocess,
        batched=True,
        remove_columns=["text", "label"],
    )

    # Загружаем модель для SequenceClassification
    model = AutoModelForSequenceClassification.from_pretrained(
        args.pretrained_model,
        num_labels=len(labels),
        id2label=id2label,
        label2id=label2id,
    )

    # Аргументы обучения (пример)
    training_args = TrainingArguments(
        output_dir="tmp",
        num_train_epochs=3,
        per_device_train_batch_size=8,
        logging_steps=10,
        save_strategy="no",
    )

    trainer = Trainer(
        model=model,
        args=training_args,
        train_dataset=tokenized,
    )

    # Обучение
    trainer.train()

    # Сохраняем модель и токенизатор локально
    out_dir = Path(args.output_dir)
    out_dir.mkdir(parents=True, exist_ok=True)
    trainer.save_model(out_dir)
    tokenizer.save_pretrained(out_dir)
    print(f"Модель и токенизатор сохранены в {out_dir}")

if __name__ == "__main__":
    main()
