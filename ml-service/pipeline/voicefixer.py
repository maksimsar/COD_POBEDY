from pathlib import Path
from voicefixer import VoiceFixer as _VF

_vf: _VF | None = None          # lazy-singleton

def _get_vf() -> _VF:
    global _vf
    if _vf is None:
        _vf = _VF()             # autodetects CPU / CUDA
    return _vf

def fix_vocals(wav_in: str, wav_out: str) -> Path:
    """
    Восстанавливает вокал и сохраняет результат в *wav_out*.
    Возвращает Path к этому файлу.
    """
    vf = _get_vf()
    vf.restore(wav_in, wav_out, cuda=False)   # у 0.1.x метод restore()
    return Path(wav_out)
