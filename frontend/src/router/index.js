import { createRouter, createWebHistory } from 'vue-router'
import VehicleListView from '../views/VehicleListView.vue'
import VehicleFormView from '../views/VehicleFormView.vue'

const routes = [
  { path: '/', name: 'vehicles', component: VehicleListView },
  { path: '/veiculos/novo', name: 'vehicle-create', component: VehicleFormView },
  {
    path: '/veiculos/:id/editar',
    name: 'vehicle-edit',
    component: VehicleFormView,
    props: true,
  },
]

export default createRouter({
  history: createWebHistory(),
  routes,
})