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
            <h1>Natural Forest <span>HTML / CSS</span></h1>
            <div class="hero-content__p">
              Creating a beautiful 3D website with a ‘lens effect’
            </div>
            <button class="button-start">Start Learning</button>
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
  import layer1 from '../assets/img/layer-1.png'
  import layer2 from '../assets/img/layer-2.png'
  import layer5 from '../assets/img/layer-5.png'
  import layer6 from '../assets/img/layer-6.png'
  
  export default {
    components: { RainEffect },
    data() {
      return { layer1, layer2, layer5, layer6 }
    }
  }
  </script>
  
  <style scoped>
  /* никаких дополнительных правок */
  </style>
  