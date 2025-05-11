<template>
  <div id="app">
    <the-navbar/>
    <main>
      <router-view/>
    </main>
  </div>
</template>

<script>
import TheNavbar from './components/TheNavbar.vue'

export default {
  components: {
    TheNavbar
  }
}
</script>

