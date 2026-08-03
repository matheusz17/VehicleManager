import axios from 'axios'

// A URL fica no .env para não depender de endereço fixo dentro dos componentes.
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
})

export default api
