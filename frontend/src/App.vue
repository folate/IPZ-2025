<script setup>
import { onMounted, computed } from "vue";
import { useAuth } from "./stores/auth";
import { useRoute } from "vue-router";
import LandingHeader from "./components/landing/LandingHeader.vue";
import LandingFooter from "./components/landing/LandingFooter.vue";
import GlobalAlertDialog from "./components/GlobalAlertDialog.vue";

const { initAuth } = useAuth();
const route = useRoute();

const showHeaderFooter = computed(() => {
  const excludedRoutes = ["login", "register"];
  return !excludedRoutes.includes(route.name);
});

onMounted(async () => {
  await initAuth();
});
</script>

<template>
  <div class="flex flex-col min-h-screen">
    <LandingHeader v-if="showHeaderFooter" />
    <router-view v-slot="{ Component, route: viewRoute }">
      <transition name="fade" mode="out-in">
        <div :key="viewRoute.name || viewRoute.path" class="flex-grow flex flex-col">
          <component :is="Component" />
        </div>
      </transition>
    </router-view>
    <LandingFooter v-if="showHeaderFooter" />
    <GlobalAlertDialog />
  </div>
</template>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
