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
                    <div class="btn-down-box">

                    </div>
                    <div class="btn-play-original">
                        <button class="btn-play-original" @click="$emit('play-original')">
                            <img src="../assets/playold.png" alt="Оригинал">
                        </button>
                        <button class="btn-play-restored" @click="$emit('play-restored')">
                            <img src="../assets/playnew.png" alt="Реставрация">
                        </button>
                    </div>
                    <div class="btn-play-restored">
                        
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
        top: 11vh;
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
        justify-content: center;
        align-items: flex-start;
        width: 100%;
        padding: 3vh 7vh;
    }

    .left, .right {
        
    }

    .left {
        display: flex;
        flex-direction: row;
        align-items: flex-start;
        width: 50%;
    }

    .arrow-back-box {
        width: 5vh;
        margin-right: 6vh;
    }

    .arrow-back-btn {
        cursor: pointer;
        background: none;
        border: none;
    }
    
    .info{
        display: flex;
        flex-direction: column;
        margin-right: 8vh;
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
        width: 50%;
        display: flex;
        flex-direction: row;
        align-items: flex-start;
        justify-content: end;
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