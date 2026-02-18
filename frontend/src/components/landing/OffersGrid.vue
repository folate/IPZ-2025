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
  <section class="offersSection">
    <h2 class="offersTitle">Oferty</h2>

    <p v-if="loading" class="offersInfo">Ładowanie...</p>
    <p v-else-if="error" class="offersError">{{ error }}</p>
    <p v-else-if="offers.length === 0" class="offersInfo">Brak ofert.</p>

    <div class="offersGrid">
      <RouterLink v-for="o in offers" :key="o.id" :to="`/offer/${o.id}`">
        <OfferCard :offer="o" />
      </RouterLink>
    </div>
  </section>
</template>
