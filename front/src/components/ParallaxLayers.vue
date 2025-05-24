<template>
  <section class="layers">
    <div class="layers__container">
      <div
        class="layers__item layer-1"
        :style="{ backgroundImage: `url(${layer1})` }"
      ></div>
      <div
        class="layers__item layer-2"
        :style="{ backgroundImage: `url(${layer2})` }"
      ></div>
      <div class="layers__item layer-3">
        <div class="hero-content">
          <h1>РТУ МИРЭА <span>КОД ПОБЕДЫ.</span></h1>
          <div class="hero-content__p">
            Команда 1с_enjoyers
          </div>
          <!-- Навигация на Introduction -->
          <button @click="goToIntroduction" class="button-start">
            Перейти к решению
          </button>
        </div>
      </div>
      <div class="layers__item layer-4">
        <RainEffect />
      </div>
      <div
        class="layers__item layer-5"
        :style="{ backgroundImage: `url(${layer5})` }"
      ></div>
      <div
        class="layers__item layer-6"
        :style="{ backgroundImage: `url(${layer6})` }"
      ></div>
    </div>
  </section>
</template>

<script>
import RainEffect from './RainEffect.vue'
import layer1      from '../assets/img/layer-1.png'
import layer2      from '../assets/img/layer-2.png'
import layer5      from '../assets/img/layer-5.png'
import layer6      from '../assets/img/layer-6.png'

export default {
  name: 'ParallaxLayers',
  components: { RainEffect },
  data() {
    return { layer1, layer2, layer5, layer6 }
  },
  methods: {
    goToIntroduction() {
      // Переход на маршрут, где рендерится Introduction.vue
      this.$router.push('/home')
    }
  }
}
</script>

<style scoped>
/* никаких дополнительных правок */
</style>
