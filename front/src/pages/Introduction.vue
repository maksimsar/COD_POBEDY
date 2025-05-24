<template>
  <div class="introduction-page">
    <!-- Анимация рупора со звуковым клином -->
    <MegaphoneSound />

    <!-- Здесь мы накладываем кнопки прямо поверх шума -->
    <div class="buttons-overlay">
      <TheButton variant="primary" to="/restoration">
        РЕСТАВРАЦИЯ
      </TheButton>
      <TheButton variant="secondary" to="/library">
        БИБЛИОТЕКА
      </TheButton>
    </div>

    <!-- Ваш текстовый блок остаётся на том же месте -->
    <section class="intro-content">
      <div class="text-block">
        <h1 class="title">
          Реставрируйте
          <span class="pink">музыкальное наследие</span>
          Великой<br />
          Отечественной Войны
        </h1>
        <p class="subtitle">
          Восстановите качество звука, узнайте текст, авторов,
          год создания и многое другое
        </p>
      </div>
    </section>
  </div>
</template>

<script setup>
import TheButton      from '@/components/Buttons.vue'
import MegaphoneSound from '@/components/MegaphoneSound.vue'
</script>
<style scoped>
.introduction-page {
  position: relative;
  width: 100%;
  min-height: 100vh;
  background: #fafafa;
  overflow: hidden;
}

/* Основной текст прижать влево */
.intro-content {
  position: relative;
  z-index: 1;
  max-width: 640px;
  margin: 40px 0 0 90px;  /* 20px от левого края, вправо — auto */
  padding: 6rem 1rem 2rem;
  box-sizing: border-box;
}

/* Заголовок и подзаголовок */
.text-block {
  margin-bottom: 2rem;
  text-align: left;
}
.title {
  font-size: 2.75rem;
  line-height: 1.2;
  margin: 0;
  color: #222;
}
.pink {
  color: var(--color-pink);
}
.subtitle {
  font-size: 1.125rem;
  line-height: 1.5;
  margin: 1rem 0 0;
  color: #555;
}

/* Кнопки по правому краю и вертикальному центру */
/* Кнопки горизонтально (в ряд), с большим gap, адаптивно растягиваются */
.buttons-overlay {
  position: absolute;
  top: 50%;                      /* центр по Y */
  right: 20px;                   /* 20px от правого края */
  transform: translateY(-50%);   /* точная центровка */
  display: flex;                 
  gap: clamp(1rem, 5vw, 4rem);   /* от 1rem до 4rem, в зависимости от ширины */
  width: clamp(300px, 40vw, 600px); /* от 300px до 600px, максимум 40% экрана */
  justify-content: space-between;/* равномерно распределить кнопки по ширине */
  z-index: 10;
}

/* Адаптивные «большие» кнопки */
.buttons-overlay .button {
  flex: 1;                       
  min-width: 0;                  /* чтобы flex:1 сжимался, когда нужно */
  padding: clamp(0.75rem, 2vw, 1.25rem) clamp(1rem, 3vw, 2rem);
  font-size: clamp(1rem, 2vw, 1.75rem);
}

</style>
