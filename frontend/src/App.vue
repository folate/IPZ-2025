<script setup>
import { onMounted } from "vue";
import { useAuth } from "./stores/auth";

const { initAuth } = useAuth();

onMounted(async () => {
  await initAuth();
});
</script>

<template>
  <router-view />
</template>
