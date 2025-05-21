# pipeline/__init__.py

# Явно делаем из директории пакет с набором утилит
from .pipeline            import process
from .click_removal       import remove_clicks
from .declip              import declip
from .fullsubnet2_denoise import denoise_fullsubnet
from .separate_vocals     import separate_vocals
from .voicefixer_wrapper  import VoiceFixer
from .mastering           import spectral_master_full
from .stt_whisper         import transcribe
from .classifier          import clf

__all__ = [
    "process",
    "remove_clicks",
    "declip",
    "denoise_fullsubnet",
    "separate_vocals",
    "VoiceFixer",
    "spectral_master_full",
    "transcribe",
    "clf",
]
