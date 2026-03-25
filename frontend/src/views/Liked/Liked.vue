<script setup>
import Container from "@/components/ui/Container.vue";
import OfferCard from "@/components/landing/OfferCard.vue";
import { Loader2 } from "lucide-vue-next";
import { ref, onMounted } from "vue";

const offers = ref([]);
const loading = ref(false);
const error = ref("");

async function fetchDetails() {
  loading.value = true;
  error.value = "";
  offers.value = [];

  const item = localStorage.getItem("FavoritesIds");
  const favoritesRaw = item ? JSON.parse(item) : [];
  const favorites = Array.isArray(favoritesRaw) ? favoritesRaw : [];

  if (favorites.length === 0) {
    loading.value = false;
    return;
  }

  try {
    const results = await Promise.all(
      favorites.map(async (fav) => {
        const id = typeof fav === "object" && fav !== null ? fav.id : fav;
        if (!id) return null;

        const res = await fetch(`/api/SellerAd/${id}`, {
          credentials: "include",
        });

        if (!res.ok) return null;
        return await res.json();
      }),
    );

    offers.value = results.filter(Boolean);
  } finally {
    loading.value = false;
  }
}

onMounted(fetchDetails);
</script>

<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20">

    <Container>
      <div class="mt-8 w-full flex flex-col gap-6">
        <div>
          <h1 class="text-3xl font-extrabold text-zinc-900 dark:text-zinc-50">Ulubione</h1>
          <p class="text-zinc-500 dark:text-zinc-400 mt-1">Twoje zapisane oferty.</p>
        </div>

        <div v-if="loading" class="flex justify-center py-12">
          <Loader2 class="h-10 w-10 text-teal-600 animate-spin" />
        </div>
        
        <div v-else-if="error" class="bg-red-50 dark:bg-red-900/10 text-red-600 dark:text-red-400 p-4 rounded-xl font-medium">
          {{ error }}
        </div>
        
        <div v-else-if="offers.length === 0" class="flex flex-col items-center justify-center py-16 text-center">
          <div class="h-24 w-24 bg-zinc-100 dark:bg-zinc-900 rounded-full flex items-center justify-center mb-4">
            <span class="text-4xl">❤️</span>
          </div>
          <h3 class="text-xl font-bold text-zinc-900 dark:text-zinc-50">Brak ulubionych ofert</h3>
          <p class="text-zinc-500 dark:text-zinc-400 mt-2 max-w-sm">Przeglądaj zlecenia i dodawaj je do ulubionych, aby mieć do nich szybki dostęp.</p>
        </div>

        <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          <RouterLink v-for="o in offers" :key="o.id" :to="`/offer/${o.id}`" class="block outline-none focus-visible:ring-2 focus-visible:ring-teal-500 rounded-2xl">
            <OfferCard :offer="o" />
          </RouterLink>
        </div>
      </div>
    </Container>
  </div>
</template>
