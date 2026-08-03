import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import './style.css'

// Cria a aplicação Vue, registra as rotas e encaixa tudo na div #app do index.html.
createApp(App).use(router).mount('#app')
