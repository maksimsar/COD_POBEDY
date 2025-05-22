# src/data.py
import os, glob, torch
from torch.utils.data import Dataset, DataLoader
import torchaudio

class DenoiseDataset(Dataset):
    def __init__(self, noisy_dir, clean_dir, sample_rate, segment_length):
        self.noisy_files = sorted(glob.glob(os.path.join(noisy_dir, "*.wav")))
        self.clean_files = sorted(glob.glob(os.path.join(clean_dir, "*.wav")))
        assert len(self.noisy_files) == len(self.clean_files)
        self.sr = sample_rate
        self.seg_len = segment_length * sample_rate

    def __len__(self): return len(self.noisy_files)

    def __getitem__(self, idx):
        noisy, _ = torchaudio.load(self.noisy_files[idx])
        clean, _ = torchaudio.load(self.clean_files[idx])
        # тут можно вырезать случайный сегмент длины self.seg_len
        return noisy, clean

def get_loaders(cfg):
    ds_train = DenoiseDataset(cfg.data.noisy_dir, cfg.data.clean_dir,
                              cfg.data.sample_rate, cfg.data.segment_length)
    return DataLoader(ds_train, batch_size=cfg.data.batch_size, shuffle=True), None
