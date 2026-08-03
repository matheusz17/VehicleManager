<template>
  <!-- Este é o layout que fica em volta de todas as páginas do sistema. -->
  <header class="app-header">
    <RouterLink to="/" class="brand">Vehicle Manager</RouterLink>
  </header>

  <main class="app-content">
    <!-- O Vue Router renderiza aqui a tela correspondente à URL atual. -->
    <RouterView />
  </main>
</template>
