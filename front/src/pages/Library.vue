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
              <label v-for="genre in allGenres" :key="'genre-' + genre">
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
              <label v-for="mood in allMoods" :key="'mood-' + mood">
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

      <div v-if="isLoading" class="loading">Загрузка...</div>
      <div v-else-if="error" class="error">{{ error }}</div>
      <div v-else-if="songs.length === 0" class="no-results">Ничего не найдено</div>
      <div v-else class="results">
        <div v-for="song in songs" :key="song.id" class="song-item">
          <h3>{{ song.title }}</h3>
          <p>{{ song.artist }} • {{ song.genre }} • {{ song.mood }}</p>
        </div>
      </div>
  </main>
  
</template>

<script>
import { debounce } from 'lodash';

export default {
  data() {
    return {
      searchQuery: '',
      showFilters: false,
      selectedGenres: [],
      selectedMoods: [],
      allGenres: ['Рок', 'Поп', 'Джаз', 'Классика', 'Народная', 'Патриотическая'],
      allMoods: ['Радостное', 'Грустное', 'Торжественное', 'Ностальгическое', 'Героическое'],
      songs: [],
      isLoading: false,
      error: null,
      debouncedSearch: debounce(this.performSearch, 500)
    };
  },
  methods: {
    toggleFilters() {
      this.showFilters = !this.showFilters;
      if (this.showFilters && (this.selectedGenres.length > 0 || this.selectedMoods.length > 0)) {
        this.performSearch();
      }
    },
    async performSearch() {
      if (!this.searchQuery && this.selectedGenres.length === 0 && this.selectedMoods.length === 0) {
        this.songs = [];
        return;
      }

      this.isLoading = true;
      this.error = null;

      try {
        // Здесь будет запрос к вашему бэкенду с Elasticsearch
        const response = await this.$axios.post('/api/songs/search', {
          query: this.searchQuery,
          genres: this.selectedGenres,
          moods: this.selectedMoods
        });

        this.songs = response.data;
      } catch (err) {
        console.error('Search error:', err);
        this.error = 'Ошибка при поиске. Пожалуйста, попробуйте позже.';
      } finally {
        this.isLoading = false;
      }
    },
    applyFilters() {
      this.performSearch();
    }
  }
};
</script>

<style scoped>

.search-container {
  max-width: 80%;
  margin: 2.5vh auto;
  position: relative;
  margin-bottom: 20px;
}

.search-input-wrapper {
  display: flex;
  align-items: center;
  border-radius: 24px;
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
  border-radius: 24px;
  padding: 0 24px;
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
      border-radius: 7px;
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

.song-item {
  padding: 12px;
  border-bottom: 1px solid #eee;
}

.song-item:hover {
  background: #f9f9f9;
}

.loading, .error, .no-results {
  padding: 20px;
  text-align: center;
  color: #666;
}

.error {
  color: #d32f2f;
}
</style>