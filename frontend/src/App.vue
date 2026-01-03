<script setup>
import { onMounted } from "vue"
import { useAuth } from "./stores/auth"

const { initAuth, state } = useAuth()

onMounted(() => {
  initAuth()
})
</script>

<template>
  <!-- Opcjonalnie: loader przy starcie -->
  <div v-if="state.loading" class="appLoader">
    Loading...
  </div>
    <router-view v-else />
</template>

<style scoped>
.appLoader
{
  height: 100vh;
  display: grid;
  place-items: center;
  font-size: 18px;
  font-weight: 800;
}
</style>
