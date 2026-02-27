<script setup>
import { ref, onMounted, onUnmounted } from "vue";
import OfferCard from "./OfferCard.vue";

const props = defineProps({
  limit: { type: Number, default: 10 },
});

const offers = ref([]);
const buyerAds = ref([]);

const loading = ref(false);
const buyerLoading = ref(false);

const error = ref("");
const buyerError = ref("");

async function loadOffers() {
  loading.value = true;
  error.value = "";

  try {
    const res = await fetch(`/api/SellerAd/all/${props.limit}`, {
      credentials: "include",
    });

    if (!res.ok) {
      error.value = `Nie udało się pobrać ofert (${res.status}).`;
      offers.value = [];
      return;
    }

    const data = await res.json();
    offers.value = Array.isArray(data) ? data : [];
  } catch {
    error.value = "Błąd sieci przy pobieraniu ofert.";
    offers.value = [];
  } finally {
    loading.value = false;
  }
}

async function loadBuyerAds() {
  buyerLoading.value = true;
  buyerError.value = "";

  try {
    const res = await fetch(`/api/BuyerAd/all/10?t=${Date.now()}`, {
      credentials: "include",
      cache: "no-store",
      headers: {
        Accept: "application/json",
        "Cache-Control": "no-cache",
        Pragma: "no-cache",
      },
    });

    if (!res.ok) {
      const text = await res.text().catch(() => "");
      buyerError.value = `Nie udało się pobrać zleceń (${res.status}). ${String(text).slice(0, 120)}`;
      return; // ważne: NIE czyścimy buyerAds
    }

    const contentType = res.headers.get("content-type") || "";
    if (!contentType.includes("application/json")) {
      const text = await res.text().catch(() => "");
      buyerError.value = `Zła odpowiedź z API (nie-JSON). ${String(text).slice(0, 120)}`;
      return;
    }

    const data = await res.json();
    buyerAds.value = Array.isArray(data) ? data : [];
  } catch {
    buyerError.value = "Błąd sieci przy pobieraniu zleceń klientów.";
  } finally {
    buyerLoading.value = false;
  }
}

function onSellerCreated() {
  loadOffers();
}

function onBuyerCreated() {
  loadBuyerAds();
}

onMounted(() => {
  loadOffers();
  loadBuyerAds();

  window.addEventListener("sellerad:created", onSellerCreated);
  window.addEventListener("buyerad:created", onBuyerCreated);
});

onUnmounted(() => {
  window.removeEventListener("sellerad:created", onSellerCreated);
  window.removeEventListener("buyerad:created", onBuyerCreated);
});
</script>

<template>
  <section class="offersSection">
    <h2 class="offersTitle">Oferty</h2>

    <p v-if="loading">Ładowanie...</p>
    <p v-else-if="error">{{ error }}</p>
    <p v-else-if="offers.length === 0">Brak ofert.</p>

    <div class="offersGrid">
      <RouterLink v-for="o in offers" :key="o.id" :to="`/offer/${o.id}`">
        <OfferCard :offer="o" />
      </RouterLink>
    </div>

    <h2 class="offersTitle" style="margin-top: 50px">Zlecenia klientów</h2>

    <p v-if="buyerLoading">Ładowanie...</p>
    <p v-else-if="buyerError">{{ buyerError }}</p>
    <p v-else-if="buyerAds.length === 0">Brak zleceń.</p>

    <div class="offersGrid">
      <div v-for="o in buyerAds" :key="o.id">
        <OfferCard :offer="o" />
      </div>
    </div>
  </section>
</template>
