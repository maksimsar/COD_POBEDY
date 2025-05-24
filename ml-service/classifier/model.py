from pathlib import Path
import torch
from transformers import AutoTokenizer, AutoModelForSequenceClassification

MODEL_NAME = "models/bert_classifier"

class BertClassifier:
    def __init__(self, device="cpu"):
        self.device = device
        self.tok    = AutoTokenizer.from_pretrained(MODEL_NAME)
        self.model  = AutoModelForSequenceClassification.from_pretrained(MODEL_NAME).to(device).eval()

    def predict(self, text: str) -> str:
        inputs = self.tok(text, return_tensors="pt", truncation=True).to(self.device)
        with torch.no_grad():
            logits = self.model(**inputs).logits.squeeze()
        label_id = int(logits.argmax())
        return self.model.config.id2label[label_id]
