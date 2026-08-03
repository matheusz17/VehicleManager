<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../services/api'

const currentYear = new Date().getFullYear()
const router = useRouter()
const route = useRoute()
const isEditing = computed(() => Boolean(route.params.id))
const loading = ref(isEditing.value)
const saving = ref(false)
const error = ref('')
const fieldErrors = ref({})

const vehicle = reactive({
  placa: '',
  marca: '',
  modelo: '',
  anoFabricacao: currentYear,
  anoModelo: currentYear,
  cor: '',
  quilometragem: 0,
  preco: null,
  combustivel: 0,
  cambio: 0,
  status: 0,
  observacoes: '',
})

const combustiveis = [
  { value: 0, label: 'Flex' },
  { value: 1, label: 'Gasolina' },
  { value: 2, label: 'Etanol' },
  { value: 3, label: 'Diesel' },
  { value: 4, label: 'GNV' },
  { value: 5, label: 'Elétrico' },
  { value: 6, label: 'Híbrido' },
]

const cambios = [
  { value: 0, label: 'Manual' },
  { value: 1, label: 'Automático' },
  { value: 2, label: 'CVT' },
  { value: 3, label: 'Automatizado' },
]

const statusOptions = [
  { value: 0, label: 'Disponível' },
  { value: 1, label: 'Reservado' },
  { value: 2, label: 'Vendido' },
]

async function loadVehicle() {
  try {
    const response = await api.get(`/veiculos/${route.params.id}`)
    const { id, criadoEm, ...vehicleData } = response.data
    Object.assign(vehicle, vehicleData)
  } catch (requestError) {
    error.value = requestError.response?.status === 404
      ? 'Veículo não encontrado.'
      : 'Não foi possível carregar o veículo.'
  } finally {
    loading.value = false
  }
}

async function saveVehicle() {
  saving.value = true
  error.value = ''
  fieldErrors.value = {}

  const payload = {
    ...vehicle,
    placa: vehicle.placa.toUpperCase(),
  }

  try {
    if (isEditing.value) {
      await api.put(`/veiculos/${route.params.id}`, payload)
    } else {
      await api.post('/veiculos', payload)
    }
    router.push('/')
  } catch (requestError) {
    const responseData = requestError.response?.data

    if (responseData?.errors) {
      fieldErrors.value = Object.fromEntries(
        Object.entries(responseData.errors).map(([field, messages]) => [
          field.charAt(0).toLowerCase() + field.slice(1),
          messages[0],
        ]),
      )
      error.value = 'Revise os campos destacados.'
    } else if (typeof responseData === 'string') {
      error.value = responseData
    } else {
      error.value = 'Não foi possível salvar o veículo. Revise os dados e tente novamente.'
    }
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  if (isEditing.value) loadVehicle()
})
</script>

<template>
  <section>
    <h1>{{ isEditing ? 'Editar veículo' : 'Novo veículo' }}</h1>
    <RouterLink to="/">Voltar para a listagem</RouterLink>

    <p v-if="loading">Carregando veículo...</p>

    <form v-else @submit.prevent="saveVehicle">
      <p v-if="error">{{ error }}</p>
      <label>
        Placa
        <input v-model="vehicle.placa" maxlength="8" pattern="[A-Za-z]{3}[0-9][A-Za-z0-9][0-9]{2}" required />
        <small v-if="fieldErrors.placa">{{ fieldErrors.placa }}</small>
      </label>

      <label>
        Marca
        <input v-model.trim="vehicle.marca" maxlength="50" required />
        <small v-if="fieldErrors.marca">{{ fieldErrors.marca }}</small>
      </label>

      <label>
        Modelo
        <input v-model.trim="vehicle.modelo" maxlength="80" required />
        <small v-if="fieldErrors.modelo">{{ fieldErrors.modelo }}</small>
      </label>

      <label>
        Ano de fabricação
        <input v-model.number="vehicle.anoFabricacao" type="number" min="1950" :max="currentYear + 1" required />
        <small v-if="fieldErrors.anoFabricacao">{{ fieldErrors.anoFabricacao }}</small>
      </label>

      <label>
        Ano do modelo
        <input v-model.number="vehicle.anoModelo" type="number" :min="vehicle.anoFabricacao" :max="currentYear + 1" required />
        <small v-if="fieldErrors.anoModelo">{{ fieldErrors.anoModelo }}</small>
      </label>

      <label>
        Cor
        <input v-model.trim="vehicle.cor" maxlength="30" required />
        <small v-if="fieldErrors.cor">{{ fieldErrors.cor }}</small>
      </label>

      <label>
        Quilometragem
        <input v-model.number="vehicle.quilometragem" type="number" min="0" required />
        <small v-if="fieldErrors.quilometragem">{{ fieldErrors.quilometragem }}</small>
      </label>

      <label>
        Preço
        <input v-model.number="vehicle.preco" type="number" min="0.01" step="0.01" required />
        <small v-if="fieldErrors.preco">{{ fieldErrors.preco }}</small>
      </label>

      <label>
        Combustível
        <select v-model.number="vehicle.combustivel">
          <option v-for="option in combustiveis" :key="option.value" :value="option.value">{{ option.label }}</option>
        </select>
        <small v-if="fieldErrors.combustivel">{{ fieldErrors.combustivel }}</small>
      </label>

      <label>
        Câmbio
        <select v-model.number="vehicle.cambio">
          <option v-for="option in cambios" :key="option.value" :value="option.value">{{ option.label }}</option>
        </select>
        <small v-if="fieldErrors.cambio">{{ fieldErrors.cambio }}</small>
      </label>

      <label>
        Status
        <select v-model.number="vehicle.status">
          <option v-for="option in statusOptions" :key="option.value" :value="option.value">{{ option.label }}</option>
        </select>
        <small v-if="fieldErrors.status">{{ fieldErrors.status }}</small>
      </label>

      <label>
        Observações
        <textarea v-model.trim="vehicle.observacoes" maxlength="500" rows="4" />
        <small v-if="fieldErrors.observacoes">{{ fieldErrors.observacoes }}</small>
      </label>

      <button type="submit" :disabled="saving">
        {{ saving ? 'Salvando...' : 'Salvar veículo' }}
      </button>
    </form>
  </section>
</template>
