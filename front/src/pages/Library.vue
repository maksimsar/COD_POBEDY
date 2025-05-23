<template>
  <main>
    <div class="search-container">
      <div class="search-input-wrapper">
        <img src="../assets/search.png">
        <input
          v-model="searchQuery"
          @input="debouncedSearch"
          @keyup.enter="performSearch"
          placeholder="Поиск"
          class="search-input"
        />
        <button @click="toggleFilters" class="filter-toggle">
          <img :class="['arrow-icon', { 'arrow-up': showFilters }]" src="../assets/arrow.png"> 
        </button>
      </div>

      <div v-if="showFilters" class="filters-panel">
        <div class="filter-section">
          <p>Жанры</p>
          <div class="filter-options">
            <label v-for="genre in availableGenres" :key="'genre-' + genre">
              <input
                type="checkbox"
                v-model="selectedGenres"
                :value="genre"
                @change="applyFilters"
              />
              {{ genre }}
            </label>
          </div>
        </div>

        <div class="filter-section">
          <p>Настроения</p>
          <div class="filter-options">
            <label v-for="mood in availableMoods" :key="'mood-' + mood">
              <input
                type="checkbox"
                v-model="selectedMoods"
                :value="mood"
                @change="applyFilters"
              />
              {{ mood }}
            </label>
          </div>
        </div>
      </div>
    </div>
  </main>
  <footer>
    <div v-if="!hasActiveFilters" class="mood-sections">
      <div v-for="mood in topMoods" :key="'mood-section-' + mood.name" class="mood-section">
        <h2 class="mood-title">{{ mood.name }}</h2>
        <div class="mood-songs">
          <SongItem 
            v-for="song in mood.songs" 
            :key="song.Id" 
            :song="song"
            @play-original="playOriginal(song)"
            @play-restored="playRestored(song)"
          />
        </div>
      </div>
    </div>

    <div v-else>
      <div v-if="filteredSongs.length === 0" class="no-results">
        Ничего не найдено
      </div>
      <div v-else class="results">
        <SongItem 
          v-for="song in filteredSongs" 
          :key="song.Id" 
          :song="song"
          @play-original="playOriginal(song)"
          @play-restored="playRestored(song)"
        />
      </div>
    </div>
  </footer>
   
  
</template>

<script>
import SongItem from '../components/SongItem.vue'
import songsData from '../assets/songs_test.json'
export default {
  components: {
    SongItem
  },
  data() {
    return {
      searchQuery: '',
      showFilters: false,
      selectedGenres: [],
      selectedMoods: [],
      allSongs: [],
      availableGenres: [],
      availableMoods: []
    }
  },
  computed: {
    hasActiveFilters() {
      return this.searchQuery !== '' || 
             this.selectedGenres.length > 0 || 
             this.selectedMoods.length > 0
    },
    
    // Топ-3 настроений с наибольшим количеством песен
    topMoods() {
      const moodCounts = {}
      
      // Считаем количество песен для каждого настроения
      this.allSongs.forEach(song => {
        if (song.Moods && Array.isArray(song.Moods)) {
          song.Moods.forEach(mood => {
            moodCounts[mood] = (moodCounts[mood] || 0) + 1
          })
        }
      })
      
      // Сортируем по количеству песен и берём топ-3
      const sortedMoods = Object.entries(moodCounts)
        .sort((a, b) => b[1] - a[1])
        .slice(0, 3)
        .map(([name]) => name)
      
      // Формируем объекты с песнями для каждого настроения
      return sortedMoods.map(mood => ({
        name: mood,
        songs: this.allSongs.filter(song => 
          song.Moods && song.Moods.includes(mood)
        ).slice(0, 6) // Берём первые 4 песни для каждого настроения
      }))
    },
    
    // Отфильтрованные песни для режима поиска
    filteredSongs() {
      return this.allSongs.filter(song => {
        // Фильтр по поисковому запросу
        const matchesSearch = this.searchQuery === '' || 
          song.Title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
          song.Artist.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
          (song.Description && song.Description.toLowerCase().includes(this.searchQuery.toLowerCase()))
        
        // Фильтр по жанрам
        const matchesGenres = this.selectedGenres.length === 0 || 
          this.selectedGenres.includes(song.Genre)
        
        // Фильтр по настроениям
        const matchesMoods = this.selectedMoods.length === 0 || 
          (song.Moods && song.Moods.some(mood => this.selectedMoods.includes(mood)))
        
        return matchesSearch && matchesGenres && matchesMoods
      })
    }
  },
  async created() {
    await this.loadSongs()
    this.extractFilters()
  },
  methods: {
    async loadSongs() {
      this.allSongs = songsData.songs
    },
    extractFilters() {
      // Получаем уникальные жанры
      this.availableGenres = [...new Set(this.allSongs.map(song => song.Genre))]
      
      // Получаем уникальные настроения из всех песен
      const allMoods = this.allSongs
        .filter(song => song.Moods)
        .flatMap(song => song.Moods)
      this.availableMoods = [...new Set(allMoods)]
    },
    toggleFilters() {
      this.showFilters = !this.showFilters
    },
    performSearch() {
      // В локальной версии просто используем computed свойства
    },
    applyFilters() {
      // В локальной версии просто используем computed свойства
    },
    formatDuration(seconds) {
      if (!seconds) return '--:--'
      const mins = Math.floor(seconds / 60)
      const secs = seconds % 60
      return `${mins}:${secs.toString().padStart(2, '0')}`
    },
    formatMoods(moods) {
      if (!moods || !moods.length) return '—'
      return moods.join(', ')
    },
    playOriginal(song) {
      console.log('Play original:', song.Title)
      // Логика воспроизведения оригинальной версии
    },
    playRestored(song) {
      console.log('Play restored:', song.Title)
      // Логика воспроизведения реставрированной версии
    }
  }
}
</script>

<style scoped>



.search-container {
  max-width: 80%;
  margin: 2.5vh auto;
  position: relative;
  margin-bottom: 1vh;
}

.search-input-wrapper {
  display: flex;
  align-items: center;
  border-radius: var(--borderradius);
  padding: 14px 24px;
  background: #d2d2d2;
}

.search-input {
  flex: 1;
  margin: 0 20px 0 20px;
  border: none;
  outline: none;
  font-size: calc(var(--buttonfontsize) - 2px);
  font-weight: bold;
  background: none;
  color: var(--h1-color);
}

.filter-toggle {
  background: none;
  border: none;
  cursor: pointer;
}

.arrow-icon {
  transition: transform 0.3s;
}

.arrow-up {
  transform: rotate(180deg);
}

.filters-panel {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: #d2d2d2;
  border-radius: var(--borderradius);
  padding: 0 24px 14px;
  margin-top: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  z-index: 100;
}

.filter-section {
  margin-bottom: 16px;
}

.filter-section p {
  font-weight: bold;
}

.filter-section input[type=checkbox] {
      --active: var(--color-dark-pink);
      --active-inner: var(--main-bg-color);
      --border: #BBC1E1;
      --background: #fff;
      --disabled: #F6F8FF;
      --disabled-inner: #E1E6F9;
      -webkit-appearance: none;
      -moz-appearance: none;
      height: 21px;
      outline: none;
      display: inline-block;
      vertical-align: top;
      position: relative;
      margin: 0;
      cursor: pointer;
      border: 1px solid var(--bc, var(--border));
      background: var(--b, var(--background));
      transition: background 0.3s, border-color 0.3s, box-shadow 0.2s;
    }
    .filter-section input[type=checkbox]:after {
      content: "";
      display: block;
      left: 0;
      top: 0;
      position: absolute;
      transition: transform var(--d-t, 0.3s) var(--d-t-e, ease), opacity var(--d-o, 0.2s);
    }
    .filter-section input[type=checkbox]:checked {
      --b: var(--active);
      --bc: var(--active);
      --d-o: .3s;
      --d-t: .6s;
      --d-t-e: cubic-bezier(.2, .85, .32, 1.2);
    }
    .filter-section input[type=checkbox]:disabled {
      --b: var(--disabled);
      cursor: not-allowed;
      opacity: 0.9;
    }
    .filter-section input[type=checkbox]:disabled:checked {
      --b: var(--disabled-inner);
      --bc: var(--border);
    }
    .filter-section input[type=checkbox]:disabled + label {
      cursor: not-allowed;
    }
    .filter-section input[type=checkbox]:hover:not(:checked):not(:disabled) {
      --bc: var(--border-hover);
    }
    .filter-section input[type=checkbox]:focus {
      box-shadow: 0 0 0 var(--focus);
    }
    .filter-section input[type=checkbox]:not(.switch) {
      width: 21px;
    }
    .filter-section input[type=checkbox]:not(.switch):after {
      opacity: var(--o, 0);
    }
    .filter-section input[type=checkbox]:not(.switch):checked {
      --o: 1;
    }
    .filter-section input[type=checkbox] + label {
      display: inline-block;
      vertical-align: middle;
      cursor: pointer;
      margin-left: 4px;
    }

    .filter-section input[type=checkbox]:not(.switch) {
      border-radius: 8px;
    }
    .filter-section input[type=checkbox]:not(.switch):after {
      width: 5px;
      height: 9px;
      border: 2px solid var(--active-inner);
      border-top: 0;
      border-left: 0;
      left: 6px;
      top: 3px;
      transform: rotate(var(--r, 20deg));
    }
    .filter-section input[type=checkbox]:not(.switch):checked {
      --r: 43deg;
    }

.filter-options {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.filter-options label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  color: var(--h1-color);
  margin-right: 10px;
}

.mood-songs{
  display: inline-grid;
  column-gap: 18px;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr;
  width: 100%;
  margin-bottom: 5vh;
}

.results{
  display: inline-grid;
  column-gap: 18px;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr;
  width: 100%;
  row-gap: 24px;
}

.mood-title{
  font-size: var(--buttonfontsize);
  font-weight: var(--h2weight);
  color: var(--color-pink);
  margin-bottom: 3vh;
}

.loading, .error, .no-results {
  padding: 20px;
  text-align: center;
  color: #666;
}

main{
  max-height: 9vh;
}

.error {
  color: #d32f2f;
}
</style>