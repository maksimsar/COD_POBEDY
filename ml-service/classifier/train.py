"""
Дообучение DistilBERT на CSV:
  text,label
"""
import argparse, pandas as pd, torch, transformers, datasets, evaluate
from transformers import AutoTokenizer, AutoModelForSequenceClassification, TrainingArguments, Trainer

def main(csv_path, out_dir="models/bert_classifier", base="distilbert-base-multilingual-cased"):
    df   = pd.read_csv(csv_path)
    lbls = sorted(df.label.unique())
    label2id = {l:i for i,l in enumerate(lbls)}
    id2label = {i:l for l,i in label2id.items()}

    ds = datasets.Dataset.from_pandas(df)
    tok = AutoTokenizer.from_pretrained(base)

    def tokenize(ex):
        return tok(ex["text"], truncation=True)

    ds = ds.map(tokenize, batched=True)
    ds = ds.train_test_split(test_size=0.1)

    model = AutoModelForSequenceClassification.from_pretrained(
        base, num_labels=len(lbls),
        id2label=id2label, label2id=label2id,
    )

    metrics = evaluate.load("accuracy")

    def compute_metrics(p):
        pred = p.predictions.argmax(-1)
        return metrics.compute(predictions=pred, references=p.label_ids)

    args = TrainingArguments(
        out_dir, num_train_epochs=3, per_device_train_batch_size=8,
        evaluation_strategy="epoch", save_strategy="epoch",
        learning_rate=2e-5, weight_decay=0.01,
    )

    trainer = Trainer(model=model, args=args, train_dataset=ds["train"],
                      eval_dataset=ds["test"], compute_metrics=compute_metrics)
    trainer.train()
    trainer.save_model(out_dir)

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("csv")
    main(**vars(parser.parse_args()))
