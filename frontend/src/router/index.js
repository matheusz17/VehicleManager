import { createRouter, createWebHistory } from 'vue-router'
import VehicleListView from '../views/VehicleListView.vue'
import VehicleFormView from '../views/VehicleFormView.vue'

// Cada objeto informa qual URL abre qual componente/tela.
const routes = [
  // A página inicial sempre mostra o estoque cadastrado.
  { path: '/', name: 'vehicles', component: VehicleListView },
  // O mesmo formulário é reutilizado para criar e editar.
  { path: '/veiculos/novo', name: 'vehicle-create', component: VehicleFormView },
  {
    path: '/veiculos/:id/editar',
    name: 'vehicle-edit',
    component: VehicleFormView,
    props: true,
  },
]

export default createRouter({
  // URLs normais, sem #; o Vite devolve a SPA quando a página é atualizada.
  history: createWebHistory(),
  routes,
})
