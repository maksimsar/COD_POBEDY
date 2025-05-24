import { createRouter, createWebHistory } from 'vue-router'
import Introduction from '../pages/Introduction.vue'
import Library from '../pages/Library.vue'
import Restoration from '../pages/Restoration.vue'
import MainPage from '../pages/MainPage.vue'
const routes = [
  {
    path: '/',
    name: 'главная',
    component: MainPage
  },
  {
    path: '/home',
    name: 'приветствие',
    component: Introduction
  },
  { 
    path: '/library',
    name: 'библиотека',
    component: Library
  },
  { 
    path: '/restoration',
    name: 'реставрация',
    component: Restoration
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
