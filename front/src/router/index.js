import { createRouter, createWebHistory } from 'vue-router'
import Introduction from '../pages/Introduction.vue'

const routes = [
  {
    path: '/',
    name: 'приветствие',
    component: Introduction
  }
  /*{ 
    path: '/about',
    name: 'about',
    component: () => import(  '../views/AboutView.vue')
  }*/
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
