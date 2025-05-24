'''#!/usr/bin/env python3
"""
Simple denoiser training script.

Usage:
    python train_denoiser.py --config /app/configs/train_denoiser.yaml
"""

import argparse, sys, yaml, time, math
from pathlib import Path

import torch
from torch.utils.data import DataLoader
from torch import nn, optim
from torch.utils.tensorboard import SummaryWriter

# ----------------- CLI & config -----------------
p = argparse.ArgumentParser()
p.add_argument("--config", help="YAML config with model & training params")
args = p.parse_args()

if not args.config:
    print("No --config supplied → skipping training.")
    sys.exit(0)

cfg_path = Path(args.config)
if not cfg_path.exists():
    raise FileNotFoundError(f"Config not found: {cfg_path}")

cfg = yaml.safe_load(cfg_path.read_text())       # • cfg.training • cfg.model …

# ----------------- Repro & device --------------
torch.manual_seed(cfg.get("seed", 42))
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

# ----------------- Dummy dataset ---------------
# замените на свой набор
class DummyDataset(torch.utils.data.Dataset):
    def __len__(self):  return 1000
    def __getitem__(self, idx):
        noisy  = torch.randn(1, 16000)
        target = torch.randn(1, 16000)
        return noisy, target

train_loader = DataLoader(
    DummyDataset(),
    batch_size = cfg["training"]["batch_size"],
    shuffle    = True,
    num_workers= 4,
)

# ----------------- Model & loss ----------------
model = nn.Sequential(
    nn.Conv1d(1, 64, kernel_size=3, padding=1),
    nn.ReLU(),
    nn.Conv1d(64, 1, kernel_size=3, padding=1)
).to(device)

loss_fn   = nn.MSELoss()
optimizer = optim.Adam(model.parameters(), lr=cfg["training"]["lr"])
scheduler = optim.lr_scheduler.StepLR(
    optimizer,
    step_size = cfg["training"]["lr_step"],
    gamma     = cfg["training"]["lr_gamma"],
)

writer = SummaryWriter(log_dir=cfg["training"].get("logdir", "./runs"))

ckpt_dir = Path(cfg["training"].get("ckpt_dir", "./checkpoints"))
ckpt_dir.mkdir(parents=True, exist_ok=True)

# ----------------- Training loop ---------------
global_step = 0
for epoch in range(cfg["training"]["epochs"]):
    model.train()
    epoch_loss = 0.0

    for noisy, clean in train_loader:
        noisy, clean = noisy.to(device), clean.to(device)

        pred  = model(noisy)
        loss  = loss_fn(pred, clean)

        optimizer.zero_grad()
        loss.backward()
        optimizer.step()

        epoch_loss += loss.item()
        writer.add_scalar("train/loss_step", loss.item(), global_step)
        global_step += 1

    # --- epoch metrics ---
    epoch_loss /= len(train_loader)
    lr_now = scheduler.get_last_lr()[0]
    writer.add_scalar("train/loss_epoch", epoch_loss, epoch)
    writer.add_scalar("train/lr", lr_now, epoch)
    print(f"Epoch {epoch+1}/{cfg['training']['epochs']}  "
          f"loss={epoch_loss:.4f}  lr={lr_now:.2e}")

    # --- save checkpoint every N epochs ---
    if (epoch + 1) % cfg["training"].get("save_every", 5) == 0:
        ckpt_path = ckpt_dir / f"epoch_{epoch+1:04d}.pt"
        torch.save({"model": model.state_dict(),
                    "optim": optimizer.state_dict(),
                    "epoch": epoch}, ckpt_path)
        print(f"Saved: {ckpt_path}")

    scheduler.step()

writer.close()
print("Training finished.") '''


#!/usr/bin/env python3
"""
Stub for train_denoiser.

Всегда сразу завершает работу со статусом 0,
чтобы команда в docker-compose не падала.
"""
import sys
print("train_denoiser.py: stub – nothing to do")
sys.exit(0)
