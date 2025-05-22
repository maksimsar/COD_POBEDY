# pipeline/classifier.py

import torch
from pathlib import Path
from transformers import AutoTokenizer, AutoModelForSequenceClassification

BASE_DIR = Path(__file__).resolve().parent.parent

class SongClassifier:
    def __init__(self, model_dir: Path = None, device: str = "cpu"):
        if model_dir is None:
            model_dir = BASE_DIR / "models" / "bert-song-classifier"
        self.model_dir = Path(model_dir)
        if not self.model_dir.exists():
            raise FileNotFoundError(f"Не найден каталог модели: {self.model_dir}")

        self.device = torch.device(device)

        self.tokenizer = AutoTokenizer.from_pretrained(
            self.model_dir,
            local_files_only=True
        )
        self.model = AutoModelForSequenceClassification.from_pretrained(
            self.model_dir,
            local_files_only=True
        ).to(self.device)
        self.model.eval()

    def predict(self, text: str) -> str:
        inputs = self.tokenizer(
            text,
            truncation=True,
            padding=True,
            return_tensors="pt"
        ).to(self.device)
        with torch.no_grad():
            logits = self.model(**inputs).logits
        idx = logits.argmax(dim=-1).item()
        return self.model.config.id2label[idx]

clf = SongClassifier(
    model_dir=BASE_DIR / "models" / "bert-song-classifier",
    device="cpu"
)
