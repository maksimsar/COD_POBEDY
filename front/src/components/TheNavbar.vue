<template>
  <nav>
    <div class="container">
      <router-link to="/" class="logo"><img src = "../assets/logo.png"></router-link>
      <div class="nav-links" v-if="!isIntroduction">
        <router-link :class="{ 'active-link': isActive('/library') }" to="/library">БИБЛИОТЕКА</router-link>
        <router-link :class="{ 'active-link': isActive('/restoration') }" to="/restoration">РЕСТАВРАЦИЯ</router-link>
      </div>
    </div>
  </nav>
</template>

<script>
import { computed } from 'vue';
import { useRoute } from 'vue-router';

export default {
  name: 'TheNavbar',
  setup() {
    const route = useRoute();

    const isActive = (path) => {
      return route.path === path;
    };

    const isIntroduction = computed(() => {
      return route.path === '/';
    });

    return {
      isActive,
      isIntroduction
    }
  }
}
</script>

<style scoped>
:root {
    --h1-color: #3b3531;
    --passive-color: #6b665f;
}

nav {
  margin: 15px 0;
  color: white;
}

.container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: var(--maxwidthsize);
  margin: 0 auto;
  padding: 0 20px;
}

.nav-links {
  display: flex;
  gap: 8vh ;
  font-size: var(--h2size);
  font-weight: var(--h2weight);
}

.nav-links a {
  color: var(--passive-color);
  padding: 5px 10px;
  border-radius: 4px;
  text-decoration: none;
}

.nav-links a:hover {
  background-color: rgba(255, 255, 255, 0.1);
  text-decoration: none;
}

.active-link {
  color: var(--h1-color) !important;
}
</style>