<script setup>
import { onMounted } from "vue"
import { useAuth } from "./stores/auth"
import { ROLES } from "./auth/roles"

const { initAuth, setMockRole, state, role } = useAuth()

onMounted(async () => {
  // ⛔ tymczasowo NIE wołamy backendu
  // await initAuth()

  // ✅ TEST RÓL – zmieniaj i sprawdzaj widoki
  setMockRole(ROLES.BUYER)
  // setMockRole(ROLES.SELLER)
  // setMockRole(ROLES.GUEST)

  console.log("AKTUALNA ROLA:", role.value)
})
</script>

<template>
  <router-view />
</template>
