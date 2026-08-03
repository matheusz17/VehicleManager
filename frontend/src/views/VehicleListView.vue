<script setup>
import { onMounted, ref } from 'vue'
import api from '../services/api'

const vehicles = ref([])
const busca = ref('')
const loading = ref(true)
const error = ref('')
const vehicleToDelete = ref(null)
const deleting = ref(false)
const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})
const numberFormatter = new Intl.NumberFormat('pt-BR')
const statusLabels = ['Disponível', 'Reservado', 'Vendido']

function formatStatus(status) {
  return statusLabels[status] ?? status
}

function requestDeletion(vehicle) {
  vehicleToDelete.value = vehicle
}

function cancelDeletion() {
  if (deleting.value) return
  vehicleToDelete.value = null
}

async function confirmDeletion() {
  if (!vehicleToDelete.value) return

  deleting.value = true
  error.value = ''

  try {
    await api.delete(`/veiculos/${vehicleToDelete.value.id}`)
    vehicleToDelete.value = null
    await loadVehicles()
  } catch {
    error.value = 'Não foi possível excluir o veículo. Tente novamente.'
  } finally {
    deleting.value = false
  }
}

async function loadVehicles() {
  loading.value = true
  error.value = ''

  try {
    const response = await api.get('/veiculos', {
      params: busca.value ? { busca: busca.value } : {},
    })
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

    <form @submit.prevent="loadVehicles">
      <label for="busca">Buscar veículo</label>
      <input
        id="busca"
        v-model.trim="busca"
        type="search"
        placeholder="Marca, modelo ou placa"
      />
      <button type="submit">Buscar</button>
    </form>

    <p v-if="loading">Carregando veículos...</p>
    <p v-else-if="error">{{ error }}</p>

    <table v-else-if="vehicles.length">
      <thead>
        <tr>
          <th>Placa</th>
          <th>Marca / Modelo</th>
          <th>Ano</th>
          <th>Cor</th>
          <th>Km</th>
          <th>Preço</th>
          <th>Status</th>
          <th>Ações</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="vehicle in vehicles" :key="vehicle.id">
          <td>{{ vehicle.placa }}</td>
          <td>{{ vehicle.marca }} {{ vehicle.modelo }}</td>
          <td>{{ vehicle.anoFabricacao }}/{{ vehicle.anoModelo }}</td>
          <td>{{ vehicle.cor }}</td>
          <td>{{ numberFormatter.format(vehicle.quilometragem) }}</td>
          <td>{{ currencyFormatter.format(vehicle.preco) }}</td>
          <td>{{ formatStatus(vehicle.status) }}</td>
          <td>
            <RouterLink :to="`/veiculos/${vehicle.id}/editar`">Editar</RouterLink>
            <button type="button" @click="requestDeletion(vehicle)">Excluir</button>
          </td>
        </tr>
      </tbody>
    </table>

    <p v-else>Nenhum veículo cadastrado.</p>

    <section v-if="vehicleToDelete">
      <p>
        Deseja excluir {{ vehicleToDelete.marca }} {{ vehicleToDelete.modelo }}
        ({{ vehicleToDelete.placa }})?
      </p>
      <button type="button" :disabled="deleting" @click="cancelDeletion">Cancelar</button>
      <button type="button" :disabled="deleting" @click="confirmDeletion">
        {{ deleting ? 'Excluindo...' : 'Confirmar exclusão' }}
      </button>
    </section>
  </section>
</template>
