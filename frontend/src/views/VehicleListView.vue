<script setup>
import { onMounted, ref } from 'vue'
import api from '../services/api'

// ref cria um valor reativo: quando ele muda, o template da tela atualiza sozinho.
// Estes refs guardam tanto os dados quanto os estados visuais da tela.
const vehicles = ref([])
const busca = ref('')
const loading = ref(true)
const error = ref('')
const vehicleToDelete = ref(null)
const deleting = ref(false)
// Os formatadores evitam montar R$ e separadores de milhar manualmente na tabela.
const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})
const numberFormatter = new Intl.NumberFormat('pt-BR')
const statusLabels = ['Disponível', 'Reservado', 'Vendido']

function formatStatus(status) {
  // A API manda o enum como número; aqui transformo no texto amigável da tabela.
  return statusLabels[status] ?? status
}

function requestDeletion(vehicle) {
  // Guardo o veículo escolhido para só excluir depois que a pessoa confirmar.
  vehicleToDelete.value = vehicle
}

function cancelDeletion() {
  // Não deixo fechar a confirmação no meio da chamada para não criar um estado confuso.
  if (deleting.value) return
  vehicleToDelete.value = null
}

async function confirmDeletion() {
  if (!vehicleToDelete.value) return

  // Enquanto exclui, travo os botões para evitar duas requisições iguais.
  deleting.value = true
  error.value = ''

  try {
    // Só atualizo a tabela depois que a API confirma a exclusão.
    await api.delete(`/veiculos/${vehicleToDelete.value.id}`)
    vehicleToDelete.value = null
    await loadVehicles()
  } catch {
    // A pessoa vê uma mensagem na tela em vez de depender do console do navegador.
    error.value = 'Não foi possível excluir o veículo. Tente novamente.'
  } finally {
    deleting.value = false
  }
}

async function loadVehicles() {
  // Este método serve tanto para abrir a página quanto para executar a busca.
  loading.value = true
  error.value = ''

  try {
    const response = await api.get('/veiculos', {
      // Se a busca estiver vazia, nem mando query string desnecessária.
      params: busca.value ? { busca: busca.value } : {},
    })
    vehicles.value = response.data
  } catch {
    error.value = 'Não foi possível carregar os veículos. Verifique se a API está em execução.'
  } finally {
    loading.value = false
  }
}

// Assim que esta tela aparece, carrego os veículos uma primeira vez.
onMounted(loadVehicles)
</script>

<template>
  <!-- v-if/v-else escolhem um estado visual por vez: carregando, erro, tabela ou vazio. -->
  <section class="card">
    <div class="page-heading">
      <div>
        <p class="eyebrow">Estoque</p>
        <h1>Veículos</h1>
      </div>
      <RouterLink to="/veiculos/novo" class="button-link">Novo veículo</RouterLink>
    </div>

    <!-- .prevent evita que o submit recarregue a página inteira. -->
    <form class="search-form" @submit.prevent="loadVehicles">
      <label for="busca">Buscar veículo</label>
      <div class="search-controls">
        <!-- v-model liga o input ao ref busca; .trim remove espaços nas pontas. -->
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
          <!-- v-for cria uma linha para cada veículo; key ajuda o Vue a identificar cada uma. -->
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
                <button type="button" class="button-text button-delete" @click="requestDeletion(vehicle)">Excluir</button>
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

    <!-- A confirmação só aparece depois que requestDeletion guarda um veículo no ref. -->
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
