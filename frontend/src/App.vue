<script setup>
import { onMounted } from "vue";
import { useAuth } from "./stores/auth";

const { initAuth, role, state } = useAuth();

onMounted(async () => {
  await initAuth();
  console.log("ROLE:", role.value);
});
</script>

<template>
  <router-view />
</template>
