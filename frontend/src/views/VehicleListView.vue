<script setup>
import { onMounted, ref } from 'vue'
import api from '../services/api'

const vehicles = ref([])
const loading = ref(true)
const error = ref('')

async function loadVehicles() {
  loading.value = true
  error.value = ''

  try {
    const response = await api.get('/veiculos')
    vehicles.value = response.data
  } catch {
    error.value = 'Não foi possível carregar os veículos. Verifique se a API está em execução.'
  } finally {
    loading.value = false
  }
}

onMounted(loadVehicles)
</script>

<template>
  <section>
    <h1>Veículos</h1>
    <RouterLink to="/veiculos/novo">Novo veículo</RouterLink>

    <p v-if="loading">Carregando veículos...</p>
    <p v-else-if="error">{{ error }}</p>

    <ul v-else-if="vehicles.length">
      <li v-for="vehicle in vehicles" :key="vehicle.id">
        {{ vehicle.placa }} — {{ vehicle.marca }} {{ vehicle.modelo }}
      </li>
    </ul>

    <p v-else>Nenhum veículo cadastrado.</p>
  </section>
</template>
