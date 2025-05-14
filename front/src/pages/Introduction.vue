<template>
  <main>
  <div class="container">
    <h1>Реставрируйте <span class="pink">музыкальное наследие</span> Великой<br> Отечественной Войны</h1>
  </div>
  <div class="container">
    <p>Восстановите качество звука, узнайте текст, авторов, год создания и многое другое</p>
  </div>
  </main>
  <div id = "buttonsback">
    <div id = "buttonsthem">
      <TheButton variant="primary" to="/restoration" @click="handleClick">РЕСТАВРАЦИЯ</TheButton>
      <TheButton variant="secondary" to="/library" @click="handleClick">БИБЛИОТЕКА</TheButton>
    </div>
  </div>
</template>

<style scoped>
  .container{
    text-align: right;
  }
  .pink{
    color: var(--color-pink);
  }
  #buttonsback{
    width: 100%;
    min-height: 60vh; /* Гарантирует минимум половину экрана */
    background-image: url("../assets/mike.png");
    background-size: cover;
  }
  #buttonsthem {
    display: flex;
    justify-content: flex-end;
    padding-right: calc((100% - var(--maxwidthsize))/2);
    margin-top: 12%;
    gap: 10%;
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


