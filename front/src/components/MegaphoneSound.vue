<template>
    <div class="megaphone-sound">
      <!-- Рупор побольше -->
      <img src="@/assets/rupor.png" class="megaphone-img" alt="Рупор" />
  
      <!-- SVG клина: от рупора до правого края -->
      <svg
        :style="svgStyle"
        :viewBox="`0 0 ${dynamicLength} ${height}`"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path :d="pathData" fill="#8B1E3F" />
      </svg>
    </div>
  </template>
  
  <script setup>
  import { ref, computed, onMounted, onUnmounted } from 'vue'
  import { createNoise2D } from 'simplex-noise'
  
  // Параметры
  const originX    = 220    // смещение рупора слева
  const originY    = -95    // Y-коорд. горла
  const marginRight= 20     // отступ справа
  const height     = 200    // высота SVG
  const angleDeg   = 60     // угол раскрытия клина
  const segments   = 80     // сегментов на гранях
  const amplitude  = 50     // шумовая амплитуда
  const speed      = 0.02   // скорость анимации
  
  // Шум и время
  const noise2D      = createNoise2D()
  const t            = ref(0)
  const windowWidth = ref(window.innerWidth)
  
  // Пересчёт ширины при resize
  function onResize() {
    windowWidth.value = window.innerWidth
  }
  onMounted(() => {
    window.addEventListener('resize', onResize)
    ;(function animate() {
      t.value += speed
      requestAnimationFrame(animate)
    })()
  })
  onUnmounted(() => {
    window.removeEventListener('resize', onResize)
  })
  
  // Динамическая длина клина до правого края
  const dynamicLength = computed(() =>
    Math.max(0, windowWidth.value - originX - marginRight)
  )
  
  // Половина угла в радианах и смещение Y для вершины
  const halfRad = (angleDeg * Math.PI/180) / 2
  const yOffset = computed(() =>
    dynamicLength.value * Math.tan(halfRad)
  )
  
  // Генерация path
  const pathData = computed(() => {
    const L   = dynamicLength.value
    const x0  = 0,           y0 = originY
    const x1  = L,           y1 = originY - yOffset.value
    const x2  = L,           y2 = originY + yOffset.value
  
    // нормали к обеим граням
    const dx1 = x1-x0, dy1=y1-y0, len1=Math.hypot(dx1,dy1)
    const nx1 =  dy1/len1, ny1=-dx1/len1
    const dx2 = x2-x0, dy2=y2-y0, len2=Math.hypot(dx2,dy2)
    const nx2 = -dy2/len2, ny2= dx2/len2
  
    let topPts = '', botPts = ''
  
    // Верхняя грань: от (0,y0) к (x1,y1)
    for (let i = 0; i <= segments; i++) {
      const tPos = i/segments
      let tx, ty
  
      if (i === 0) {
        // фиксируем начало
        tx = x0; ty = y0
      } else {
        // базовая точка
        const bx = x0 + dx1 * tPos
        const by = y0 + dy1 * tPos
        // два шума разной частоты
        const n1 = noise2D(tPos * 2,       t.value)
        const n2 = noise2D(tPos * 10, t.value * 0.5)
        const n  = n1 * 0.6 + n2 * 0.4
        tx = bx + nx1 * n * amplitude
        ty = by + ny1 * n * amplitude
      }
  
      topPts += (i === 0 ? 'M' : 'L') + `${tx.toFixed(1)},${ty.toFixed(1)} `
    }
  
    // Прямая база (вершина x1->x2)
    topPts += `L${x2.toFixed(1)},${y2.toFixed(1)} `
  
    // Нижняя грань: обратный ход от (x2,y2) к (0,y0)
    for (let i = segments; i >= 0; i--) {
      const tPos = i/segments
      let tx, ty
  
      if (i === 0) {
        tx = x0; ty = y0
      } else {
        const bx = x0 + dx2 * tPos
        const by = y0 + dy2 * tPos
        const n1 = noise2D(tPos * 2,        t.value + 50)
        const n2 = noise2D(tPos * 10, t.value * 0.5 + 50)
        const n  = n1 * 0.6 + n2 * 0.4
        tx = bx + nx2 * n * amplitude
        ty = by + ny2 * n * amplitude
      }
  
      botPts += `L${tx.toFixed(1)},${ty.toFixed(1)} `
    }
  
    return topPts + botPts + 'Z'
  })
  
  // Стили для svg, чтобы он прилипал к правому краю
  const svgStyle = computed(() => ({
    position: 'absolute',
    bottom: '40px',
    left:   `${originX}px`,
    width:  `${dynamicLength.value}px`,
    height: `${height}px`,
    overflow: 'visible',
    zIndex: 2          // svg ниже кнопок
  }))
  </script>
  
  <style scoped>
  .megaphone-sound {
    position: absolute;
    bottom: 20px;
    left: 20px;
    pointer-events: none;
    z-index: 1;
  }
  .megaphone-img {
    position: relative;
    bottom: 0;
    left: -50px;
    width: 300px;    /* рупор ещё больше */
    z-index: 2;
  }
  </style>
  