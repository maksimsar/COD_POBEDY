<!-- src/pages/MainPage.vue -->
<template>
  <div>
    <div class="logo" :style="{ backgroundImage: `url(${logo})` }"></div>
    <ParallaxLayers />
  </div>
</template>

<script>
// импортируем из папки components
import ParallaxLayers from '../components/ParallaxLayers.vue'
// логотип лежит в src/assets/img/
// убедитесь, что файл реально называется logotip.svg
import logo from '../assets/logo.png'

export default {
  name: 'MainPage',
  components: {
    ParallaxLayers
  },
  mounted() {
    document.addEventListener('mousemove', e => {
      document.documentElement.style.setProperty(
        '--move-x',
        `${(e.clientX - window.innerWidth / 2) * -0.005}deg`
      )
      document.documentElement.style.setProperty(
        '--move-y',
        `${(e.clientY - window.innerHeight / 2) * 0.01}deg`
      )
    })
  }
}
</script>

<style>
/* все остальные стили — в main.css */
</style>
