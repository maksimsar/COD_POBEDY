<template>
  <div class="container">
    <h1>Реставрируйте <span class="pink">музыкальное наследие</span> Великой<br> Отечественной Войны</h1>
  </div>
  <div class="container">
    <p>Восстановите качество звука, узнайте текст, авторов, год создания и многое другое</p>
  </div>
  <div>
    <TheButton variant="primary" to="/restoration" @click="handleClick">РЕСТАВРАЦИЯ</TheButton>
    <TheButton variant="secondary" to="/library" @click="handleClick">БИБЛИОТЕКА</TheButton>
  </div>
</template>

<style scoped>
  .container{
    text-align: right;
  }
  .pink{
    color: var(--color-pink);
  }
</style>

<script>
  import TheButton from "../components/Buttons.vue"

  export default {
    components: {
      TheButton
    },
    methods: {
      handleClick(){}
    }
  }
</script>


