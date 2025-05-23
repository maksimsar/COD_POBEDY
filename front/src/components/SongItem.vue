<template>
<div class="song-item">
    <div class="gradient-overlay"></div>
    <div class="gradient-overlaydark"></div>
    <div class="song-actions">
        <button class="action-btn original" @click="$emit('play-original')">
            <img class='play' src="../assets/playold.png" alt="Оригинал">
        </button>
        <button class="action-btn resto" @click="$emit('play-restored')">
            <img class='play' src="../assets/playnew.png" alt="Рестоврация">
        </button>
    </div>
    <div class="songInfo">
        <div class="songTitleDiv">
            <btn class="title-btn">
                <div class="title-wrapper">
                    <span class="songTitle">{{ song.Title }}</span>
                    <span class="songTitle">{{ song.Title }}</span> <!-- Копия текста -->
                </div>
            </btn>
        </div>
        <div class="songInfoSub">
            <p class="artist">{{ song.Artist }}</p>
            <p class="time">{{ formatDuration(song.DurationSec) }}</p>
        </div>
    </div>
</div>
</template>

<script>
export default {
  name: 'SongItem',
  props: {
    song: {
      type: Object,
      required: true
    },
    backgroundImage: {
      type: String,
      default: require('../assets/song1.png')
    }
  },
  computed: {
    songStyle() {
      return {
        backgroundImage: `url(${this.backgroundImage})`,
        backgroundPosition: 'var(--mikeposition)',
        backgroundSize: 'var(--mikesize)',
        backgroundRepeat: 'no-repeat'
      }
    }
  },
  methods: {
    formatDuration(seconds) {
      if (!seconds) return '--:--'
      const mins = Math.floor(seconds / 60)
      const secs = seconds % 60
      return `${mins}:${secs.toString().padStart(2, '0')}`
    }
  }
}
</script>

<style scoped>
.gradient-overlay {
  border-radius: var(--borderradius);
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 100%;
  background: linear-gradient(
    to bottom,
    rgba(146, 5, 54, 0.678) 0%,
    rgba(158, 5, 59, 0.178) 60%,
    rgba(255, 107, 158, 0) 90%
  );
  opacity: 0;
  transition: opacity 0.3s ease;
  pointer-events: none;
  z-index: -1;
}

.gradient-overlaydark {
  border-radius: var(--borderradius);
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 100%;
  background: linear-gradient(
    to bottom,
    rgba(43, 42, 42, 0) 0%,
    rgba(58, 57, 57, 0.178) 40%,
    rgb(32, 31, 32) 80%
  );
}

.song-item {
  background-image: url("../assets/song1.png");
    background-position: var(--mikeposition);
    background-size: var(--mikesize);
    background-repeat: no-repeat;
  border-radius: var(--borderradius);
  width: 19vh;
  height: 19vh;
  border-bottom: 1px solid #eee;
  text-align: left;
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
  padding: 12px;
  border-bottom: 1px solid #eee;
  position: relative; /* Для позиционирования */
}

.song-item:hover {
  transform: translateY(-4px) scale(1.04);
  z-index: 1;
  transition: all 0.3s ease;
}

.song-item:hover .song-actions {
  opacity: 1;
  transform: translateY(0);
}

.song-item:hover .gradient-overlay {
  opacity: 1;
}

.song-actions {
  position: absolute;
  top: 3.5vh;
  right: 4.0vh;
  display: flex;
  gap: 7vh;
  opacity: 0;
  transform: translateY(-10px);
  transition: all 0.3s ease;
  z-index: 2;
}


.play{
 height: 260%;
}

.action-btn {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: none;
  background: transparent;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;
}

.songInfo{
  color:#e9e9e9;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  
}

.songInfoSub{
  font-size: 0.9em;
  display: flex;
  align-items: center;
  gap: 8px; /* Расстояние между элементами */
  font-size: 0.8rem;
}

.songTitleDiv{
  position: relative;
  max-width: 100%;
  overflow: hidden;
  mask-image: linear-gradient(
    to right,
    black calc(100% - 40px),
    transparent
  );
  margin-bottom: 0;
}

.artist, .time{
  margin-top: 0;
  margin-bottom: 0.5vh;
}

.artist{
  overflow: hidden;
  mask-image: linear-gradient(
    to right,
    black calc(100% - 15px),
    transparent
  );
}

.title-wrapper {
  display: inline-flex; /* Размещаем копии текста в строку */
  white-space: nowrap;
}

.songTitle{
  font-size: 1.17em;
  font-weight: bold;
  white-space: nowrap;
  margin: 0;
  padding-right: 40px; /* Для градиентного ухода */
  display: inline-block; /* Чтобы transform работал правильно */
  animation: none; /* По умолчанию анимация выключена */
}

.song-item:hover .title-wrapper {
  animation: scrollTitle 30s linear infinite;
}

.song-item:hover .songTitle {
  animation: scrollTitle 4s linear infinite;
  mask-image: none; /* Убираем маску при наведении */
}

@keyframes scrollTitle {
  0% {
    transform: translateX(0);
  }
  100% {
    transform: translateX(calc(-100% + 100px)); /* Подберите значение под ваш дизайн */
  }
}
</style>