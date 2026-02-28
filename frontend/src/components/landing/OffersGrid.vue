<script setup>
import { ref, onMounted, onUnmounted } from "vue";
import OfferCard from "./OfferCard.vue";

const props = defineProps({
  limit: { type: Number, default: 10 },
});

const offers = ref([]);
const loading = ref(false);
const error = ref("");

async function loadOffers() {
  loading.value = true;
  error.value = "";

  try {
    const res = await fetch(`/api/SellerAd/all/${props.limit}`, {
      credentials: "include",
    });

    if (!res.ok) {
      offers.value = [];
      error.value = `Nie udało się pobrać ofert (${res.status}).`;
      return;
    }

    const data = await res.json();
    offers.value = Array.isArray(data) ? data : [];
  } catch {
    offers.value = [];
    error.value = "Błąd sieci przy pobieraniu ofert.";
  } finally {
    loading.value = false;
  }
}

function onCreated() {
  loadOffers();
}

onMounted(() => {
  loadOffers();
  window.addEventListener("sellerad:created", onCreated);
});

onUnmounted(() => {
  window.removeEventListener("sellerad:created", onCreated);
});
</script>

<template>
  <section class="container mx-auto px-4 sm:px-6 py-12 sm:py-16 w-full">
    <div class="flex items-center justify-between mb-8">
      <h2 class="text-2xl sm:text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">
        Polecane Oferty
      </h2>
    </div>

    <div v-if="loading" class="flex justify-center items-center py-20">
      <div class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-teal-600"></div>
    </div>
    
    <div v-else-if="error" class="bg-red-50 dark:bg-red-900/20 text-red-600 dark:text-red-400 p-6 rounded-2xl text-center border border-red-100 dark:border-red-900/30">
      <p class="font-medium">{{ error }}</p>
    </div>
    
    <div v-else-if="offers.length === 0" class="bg-zinc-50 dark:bg-zinc-900/50 p-12 text-center rounded-2xl border border-zinc-200 dark:border-zinc-800">
      <p class="text-zinc-500 dark:text-zinc-400 text-lg">Brak dostępnych ofert w tym momencie.</p>
    </div>

    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      <OfferCard v-for="o in offers" :key="o.id" :offer="o" />
    </div>
  </section>
</template>
