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
    min-height: var(--introminheight); /* Гарантирует минимум половину экрана */
    background-image: url("../assets/mike.png");
    background-position: var(--mikeposition);
    background-size: var(--mikesize);
    background-repeat: no-repeat;
  }
  #buttonsthem {
    display: flex;
    justify-content: var(--btnpos);
    padding-right: var(--buttonright);
    margin-top: var(--introductionbuttinstop);
    gap: var(--buttonsgap);
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


