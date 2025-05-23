<template>
    <div class="modal-song">
        <div class="song-info-back">
            <div class="song-info">
                <div class="left">
                    <div class = "arrow-back-box" @click="$emit('close')">
                        <button class = "arrow-back-btn">
                            <img class="arrow-back" src="../assets/arrow_back.png" alt="Назад"></img>
                        </button>
                    </div>
                    <div class="info">
                        <h1 class="title">{{ song.Title }}</h1>
                        <h2 class="artist">{{ song.Artist }}</h2>
                        <h2 class="year">{{ song.Year }}</h2>
                    </div>
                    <div class="duration-box"> <h2 class="duration">{{ formatDuration(song.DurationSec) }}</h2> </div>
                </div>
                <div class="right">
                    <div class="btn-play">
                        <button @click="toggleFreq" class="btn-down">
                            <img :class="['arrow-down-icon', { 'arrow-up': showFreq }]" src="../assets/arrowwhite.png"> 
                        </button>
                        <button class="btn-play-original" @click="$emit('play-original')">
                            <img class="arrow-play-original-icon" src="../assets/playold.png" alt="Оригинал">
                        </button>
                        <button class="btn-play-restored" @click="$emit('play-restored')">
                            <img class="arrow-play-restored-icon" src="../assets/playnew.png" alt="Реставрация">
                        </button>
                    </div>
                    <div class="song-img" :alt="song.Title">
                        <img src="../assets/song1.png">
                    </div>
                </div>
            </div>
        </div>      
    </div>
    <div class="categories">

    </div>
    <div class="lyrics">
        <h2 class="title">{{ song.Title }}</h2>
        <h2 class="text">{{ song.lyrics }}</h2>
    </div>
</template>

<script>
export default {
  name: 'TrackModal',
  props: {
    song: {
      type: Object,
      required: true
    }
  },
  methods: {
    formatDuration(seconds) {
      if (!seconds) return '--:--';
      const mins = Math.floor(seconds / 60);
      const secs = seconds % 60;
      return `${mins}:${secs.toString().padStart(2, '0')}`;
    }
  },
  mounted() {
    // Блокируем прокрутку при монтировании
    document.body.style.overflow = 'hidden';
    document.documentElement.style.overflow = 'hidden';
  },
  
  beforeUnmount() {
    // Восстанавливаем прокрутку при закрытии
    document.body.style.overflow = '';
    document.documentElement.style.overflow = '';
  }
}
</script>

<style scoped>
    .modal-song{
        position: fixed;
        top: 12vh;
        left: 0;
        right: 0;
        bottom: 0;
        display: flex;
        align-items: flex-start;
        flex-direction: column;
        z-index: 2000;
        background: var(--main-bg-color);
        width: 100%;
        max-width: 100%;
        max-height: 100%;
        overflow-y: auto;
    }
    
    .song-info-back{
        background: var(--color-dark-pink);
        width: 100%;
    }
    .song-info{
        color: var(--color-white);
        display: inline-flex;
        flex-direction: row;
        justify-content: space-between;
        width: 100%;
        padding: 3vh 0;
        margin: 0 auto;
    }

    .left {
        display: flex;
        flex-direction: row;
        align-items: flex-start;
        width: 45%;
        margin-left: 9vh;
    }

    .arrow-back-box {
        width: 5vh;
        margin-right: 6vh;
        flex-shrink: 0;
    }

    .arrow-back-btn {
        cursor: pointer;
        background: none;
        border: none;
    }
    
    .info{
        display: flex;
        flex-direction: column;
        margin-right: 14vh;
    }

    .title {
        margin: 0 0 5vh 0;
    }

    .artist {
        margin: 0;
        font-weight: normal;
    }
    .year{
        margin: 0;
        font-weight: normal;
    }

    .duration{
        margin: 0;
        font-weight: normal;
    }

    .right{
        width: 45%;
        display: flex;
        flex-direction: row;
        justify-content: end;
        gap: 8vh;
        margin-right: 10vh;
    }

    .btn-play{
        display: flex;
        margin-top: auto;
        gap: 1vh;
    }

    .btn-down, .btn-play-original, .btn-play-restored{
        background: none;
        border: none;
    }

    .arrow-down-icon {
        transition: transform 0.3s;
        width: 7vh;
    }

    .arrow-up {
        transform: rotate(180deg);
    }

    .arrow-play-original-icon, .arrow-play-restored-icon{
        width: 11vh;
        object-fit: contain;
    }

    .song-img img {
        width: 100%;
        max-width: 300px;
        border-radius: var(--borderradius);
        
    }
        
    .lyrics {
        font-weight: normal;        
    }

</style>