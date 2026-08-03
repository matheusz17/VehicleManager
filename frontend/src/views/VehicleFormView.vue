<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../services/api'

// O limite dos anos acompanha o calendário sem precisar atualizar o front manualmente.
const currentYear = new Date().getFullYear()
// useRouter navega por código depois de salvar; useRoute lê o id presente na URL.
const router = useRouter()
const route = useRoute()
// A presença do id na rota diz se o mesmo formulário está criando ou editando.
const isEditing = computed(() => Boolean(route.params.id))
// Só preciso carregar dados antes de exibir o formulário quando estou editando.
const loading = ref(isEditing.value)
const saving = ref(false)
const error = ref('')
// Aqui guardo mensagens como { placa: "..." } para mostrar embaixo do campo certo.
const fieldErrors = ref({})

// O objeto reativo é ligado diretamente aos campos do formulário.
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

// Os valores numéricos precisam bater com a ordem dos enums C# enviados pela API.
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
    // Na edição, busco os dados existentes antes de mostrar o formulário.
    const response = await api.get(`/veiculos/${route.params.id}`)
    // Id e CriadoEm são controlados no backend, então não entram no payload de edição.
    const { id, criadoEm, ...vehicleData } = response.data
    Object.assign(vehicle, vehicleData)
  } catch (requestError) {
    // Dou uma explicação diferente quando a URL aponta para um veículo que não existe.
    error.value = requestError.response?.status === 404
      ? 'Veículo não encontrado.'
      : 'Não foi possível carregar o veículo.'
  } finally {
    loading.value = false
  }
}

async function saveVehicle() {
  // Limpo os erros antigos a cada nova tentativa de salvar.
  saving.value = true
  error.value = ''
  fieldErrors.value = {}

  // Deixo a placa maiúscula antes de enviar, no mesmo formato validado pela API.
  const payload = {
    ...vehicle,
    placa: vehicle.placa.toUpperCase(),
  }

  try {
    if (isEditing.value) {
      // PUT preserva o mesmo registro identificado pela rota.
      await api.put(`/veiculos/${route.params.id}`, payload)
    } else {
      // Sem id na rota significa cadastro novo, então faço POST.
      await api.post('/veiculos', payload)
    }
    // Voltando para a lista, já enxergo o resultado da criação ou edição.
    router.push('/')
  } catch (requestError) {
    const responseData = requestError.response?.data

    if (responseData?.errors) {
      // O ASP.NET devolve os erros de Data Annotations por campo; adapto a chave para o Vue.
      fieldErrors.value = Object.fromEntries(
        Object.entries(responseData.errors).map(([field, messages]) => [
          field.charAt(0).toLowerCase() + field.slice(1),
          messages[0],
        ]),
      )
      error.value = 'Revise os campos destacados.'
    } else if (typeof responseData === 'string') {
      // Regras de negócio do service retornam uma mensagem simples, como placa duplicada.
      error.value = responseData
    } else {
      error.value = 'Não foi possível salvar o veículo. Revise os dados e tente novamente.'
    }
  } finally {
    saving.value = false
  }
}

// O cadastro novo não busca nada; a edição chama a API assim que a tela monta.
onMounted(() => {
  if (isEditing.value) loadVehicle()
})
</script>

<template>
  <!-- O título reaproveita o mesmo componente e muda conforme exista id na rota. -->
  <section class="card">
    <div class="page-heading">
      <div>
        <p class="eyebrow">Estoque</p>
        <h1>{{ isEditing ? 'Editar veículo' : 'Novo veículo' }}</h1>
      </div>
      <RouterLink to="/" class="back-link">Voltar para a listagem</RouterLink>
    </div>

    <p v-if="loading" class="feedback" role="status">Carregando veículo...</p>

    <!-- Cada v-model mantém o input e o objeto vehicle sincronizados nos dois sentidos. -->
    <form v-else class="vehicle-form" @submit.prevent="saveVehicle">
      <p v-if="error" class="feedback feedback-error" role="alert">{{ error }}</p>
      <label>
        Placa
        <input v-model="vehicle.placa" maxlength="8" pattern="[A-Za-z]{3}[0-9][A-Za-z0-9][0-9]{2}" required />
        <!-- Se a API rejeitar o campo, a mensagem aparece exatamente abaixo dele. -->
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

      <div class="form-actions">
        <RouterLink to="/" class="button-link button-secondary">Cancelar</RouterLink>
        <button type="submit" :disabled="saving">
          {{ saving ? 'Salvando...' : 'Salvar veículo' }}
        </button>
      </div>
    </form>
  </section>
</template>
