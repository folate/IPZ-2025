<script setup>
import LandingHeader from "@/components/landing/LandingHeader.vue";
import OfferCard from "@/components/landing/OfferCard.vue";
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
  <main>
    <LandingHeader />

    <section class="offersSection">
      <h2 class="offersTitle">Ulubione</h2>

      <p v-if="loading" class="offersInfo">Ładowanie...</p>
      <p v-else-if="error" class="offersError">{{ error }}</p>
      <p v-else-if="offers.length === 0" class="offersInfo">
        Brak ulubionych ofert.
      </p>

      <div class="offersGrid">
        <RouterLink v-for="o in offers" :key="o.id" :to="`/offer/${o.id}`">
          <OfferCard :offer="o" />
        </RouterLink>
      </div>
    </section>
  </main>
</template>
