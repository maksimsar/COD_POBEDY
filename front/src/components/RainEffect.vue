<template>
  <canvas ref="canvas" class="rain"></canvas>
</template>

<script>
export default {
  name: 'RainEffect',
  mounted() {
    const canvas = this.$refs.canvas
    const ctx = canvas.getContext('2d')

    // Устанавливает размеры канваса
    function resizeCanvas() {
      canvas.width = window.innerWidth
      canvas.height = window.innerHeight
    }
    resizeCanvas()

    // Утилита для случайного числа в диапазоне [min, max)
    function randomNum(min, max) {
      return Math.random() * (max - min) + min
    }

    // Конструктор капли
    function RainDrop(x, y, length, velocity, opacity) {
      this.x = x
      this.y = y
      this.length = length
      this.velocity = velocity
      this.opacity = opacity

      this.draw = function() {
        ctx.beginPath()
        ctx.moveTo(this.x, this.y)
        ctx.lineTo(this.x, this.y - this.length)
        ctx.lineWidth = 1
        ctx.strokeStyle = `rgba(255, 255, 255, ${this.opacity})`
        ctx.stroke()
      }

      this.update = function() {
        if (this.y > canvas.height + 20) {
          // если ушла за экран — возвращаем на верх
          this.y = -this.length
        } else {
          this.y += this.velocity
        }
        this.draw()
      }
    }

    let rainArray = []

    // Создаём нужное число капель по всему экрану
    function initRain() {
      rainArray = []
      for (let i = 0; i < 500; i++) {
        const x = Math.random() * canvas.width
        const y = Math.random() * canvas.height    // раскидываем по всему экрану
        const length = randomNum(10, 20)            // длина 10–20px
        const velocity = randomNum(4, 15)           // скорость 4–15px/кадр
        const opacity = randomNum(0.3, 0.7)         // прозрачность 0.3–0.7
        rainArray.push(new RainDrop(x, y, length, velocity, opacity))
      }
    }
    initRain()

    // Анимация
    function animate() {
      requestAnimationFrame(animate)
      ctx.clearRect(0, 0, canvas.width, canvas.height)
      rainArray.forEach(drop => drop.update())
    }
    animate()

    // При изменении размера окна — ресайзим канвас и пересоздаём капли
    window.addEventListener('resize', () => {
      resizeCanvas()
      initRain()
    })
  }
}
</script>

<style scoped>
canvas.rain {
  display: block;
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}
</style>
