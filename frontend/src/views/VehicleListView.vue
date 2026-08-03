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
  <section class="card">
    <div class="page-heading">
      <div>
        <p class="eyebrow">Estoque</p>
        <h1>Veículos</h1>
      </div>
      <RouterLink to="/veiculos/novo" class="button-link">Novo veículo</RouterLink>
    </div>

    <form class="search-form" @submit.prevent="loadVehicles">
      <label for="busca">Buscar veículo</label>
      <div class="search-controls">
        <input
          id="busca"
          v-model.trim="busca"
          type="search"
          placeholder="Marca, modelo ou placa"
        />
        <button type="submit">Buscar</button>
      </div>
    </form>

    <p v-if="loading" class="feedback" role="status">Carregando veículos...</p>
    <p v-else-if="error" class="feedback feedback-error" role="alert">{{ error }}</p>

    <div v-else-if="vehicles.length" class="table-wrapper">
      <table>
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
            <td><strong>{{ vehicle.placa }}</strong></td>
            <td>{{ vehicle.marca }} {{ vehicle.modelo }}</td>
            <td>{{ vehicle.anoFabricacao }}/{{ vehicle.anoModelo }}</td>
            <td>{{ vehicle.cor }}</td>
            <td>{{ numberFormatter.format(vehicle.quilometragem) }}</td>
            <td>{{ currencyFormatter.format(vehicle.preco) }}</td>
            <td><span class="status-badge">{{ formatStatus(vehicle.status) }}</span></td>
            <td>
              <div class="row-actions">
                <RouterLink :to="`/veiculos/${vehicle.id}/editar`">Editar</RouterLink>
                <button type="button" class="button-danger button-text" @click="requestDeletion(vehicle)">Excluir</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="empty-state">
      <h2>Nenhum veículo cadastrado</h2>
      <p>Cadastre o primeiro veículo para começar a organizar o estoque.</p>
    </div>

    <section v-if="vehicleToDelete" class="delete-confirmation" aria-live="polite">
      <p class="confirmation-title">Confirmar exclusão</p>
      <p>
        Deseja excluir {{ vehicleToDelete.marca }} {{ vehicleToDelete.modelo }}
        ({{ vehicleToDelete.placa }})?
      </p>
      <div class="confirmation-actions">
        <button type="button" class="button-secondary" :disabled="deleting" @click="cancelDeletion">Cancelar</button>
        <button type="button" class="button-danger" :disabled="deleting" @click="confirmDeletion">
          {{ deleting ? 'Excluindo...' : 'Excluir veículo' }}
        </button>
      </div>
    </section>
  </section>
</template>
