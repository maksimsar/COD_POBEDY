import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import './assets/css/main.css'
import store from './store'
import './theme.css'

createApp(App).use(store).use(router).mount('#app')
