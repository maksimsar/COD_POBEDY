"""
pipeline.declip
───────────────
• Пытаемся найти Declipper из репо DDD:
    1) /opt/ddd/declip.py               (старые версии)
    2) /opt/ddd/src/declip.py           (новые)
• Если не нашли → просто копируем входной WAV.
"""

from __future__ import annotations
from pathlib import Path
import importlib.util, shutil, sys

# ------------------------------------------------------------------ #
#  try to load declip.py from several known locations
def _load_declipper() -> "type[object] | None":
    candidates = [
        Path("/opt/ddd/declip.py"),
        Path("/opt/ddd/src/declip.py"),
        Path(__file__).resolve().parent / "declip_local.py",  # ← your local fallback, optional
    ]
    for file in candidates:
        if file.exists():
            spec = importlib.util.spec_from_file_location("ddd_declip", file)
            if spec and spec.loader:
                mod = importlib.util.module_from_spec(spec)
                sys.modules["ddd_declip"] = mod
                spec.loader.exec_module(mod)
                if hasattr(mod, "Declipper"):
                    return mod.Declipper
    return None


_DeclipperCls = _load_declipper()

# ------------------------------------------------------------------ #
def declip(wav_in: Path, wav_out: Path) -> Path:
    """
    • Если Declipper найден — обрабатываем файл.
    • Иначе просто копируем.
    """
    if _DeclipperCls is None:
        # fallback: no declip -> copy input
        shutil.copy(wav_in, wav_out)
        return wav_out

    dec = _DeclipperCls()
    tmp_out = dec.process(str(wav_in))        # путь, который вернёт DDD
    Path(tmp_out).rename(wav_out)
    return wav_out
