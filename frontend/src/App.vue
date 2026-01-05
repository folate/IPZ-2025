<script setup>
import { onMounted } from "vue"
import { useAuth } from "./stores/auth"
import { ROLES } from "./auth/roles"

const { initAuth, setMockRole, state, role } = useAuth()

onMounted(async () => {
  // await initAuth()

  setMockRole(ROLES.BUYER)
  // setMockRole(ROLES.SELLER)
  // setMockRole(ROLES.GUEST)

  console.log("AKTUALNA ROLA:", role.value)
})
</script>

<template>
  <router-view />
</template>
