<template>
    <canvas ref="canvas" class="rain"></canvas>
  </template>
  
  <script>
  export default {
    name: 'RainEffect',
    mounted() {
      const canvas = this.$refs.canvas
      canvas.width = window.innerWidth
      canvas.height = window.innerHeight
      const c = canvas.getContext('2d')
  
      function randomNum(max, min) {
        return Math.floor(Math.random() * max) + min
      }
  
      function RainDrops(x, y, endy, velocity, opacity) {
        this.x = x
        this.y = y
        this.endy = endy
        this.velocity = velocity
        this.opacity = opacity
        this.draw = function() {
          c.beginPath()
          c.moveTo(this.x, this.y)
          c.lineTo(this.x, this.y - this.endy)
          c.lineWidth = 1
          c.strokeStyle = 'rgba(255, 255, 255, ' + this.opacity + ')'
          c.stroke()
        }
        this.update = function() {
          const rainEnd = window.innerHeight + 100
          if (this.y >= rainEnd) {
            this.y = this.endy - 100
          } else {
            this.y += this.velocity
          }
          this.draw()
        }
      }
  
      const rainArray = []
      for (let i = 0; i < 140; i++) {
        const x = Math.random() * window.innerWidth
        const y = Math.random() * -500
        const h = randomNum(10, 2)
        const v = randomNum(20, 0.2)
        const o = Math.random() * 0.55
        rainArray.push(new RainDrops(x, y, h, v, o))
      }
  
      function animate() {
        requestAnimationFrame(animate)
        c.clearRect(0, 0, window.innerWidth, window.innerHeight)
        rainArray.forEach(drop => drop.update())
      }
      animate()
  
      window.addEventListener('resize', () => {
        canvas.width = window.innerWidth
        canvas.height = window.innerHeight
      })
    }
  }
  </script>
  
  <style scoped>
  canvas.rain {
    display: block;
  }
  </style>
  