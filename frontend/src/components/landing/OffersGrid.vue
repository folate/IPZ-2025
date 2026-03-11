<script setup>
import { ref, onMounted, onUnmounted, computed } from "vue";
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

const searchTerm = ref("");

function norm(s) {
  return String(s ?? "")
    .toLowerCase()
    .trim();
}

const filteredOffers = computed(() => {
  const q = norm(searchTerm.value);
  if (!q) return offers.value;

  return offers.value.filter((o) => {
    return (
      norm(o.title).includes(q) ||
      norm(o.description).includes(q) ||
      norm(o.category).includes(q) ||
      norm(o.freelancer).includes(q)
    );
  });
});

const filteredBuyerAds = computed(() => {
  const q = norm(searchTerm.value);
  if (!q) return buyerAds.value;

  return buyerAds.value.filter((o) => {
    const buyer =
      (typeof o.buyerName === "string" && o.buyerName.trim() && o.buyerName) ||
      (typeof o.buyer === "string" && o.buyer.trim() && o.buyer) ||
      "";

    return (
      norm(o.title).includes(q) ||
      norm(o.description).includes(q) ||
      norm(o.category).includes(q) ||
      norm(buyer).includes(q)
    );
  });
});

async function loadOffers() {
  loading.value = true;
  error.value = "";

  try {
    const q = norm(searchTerm.value);
    const url = q
      ? `/api/Search?search=${encodeURIComponent(q)}`
      : `/api/SellerAd/all/${props.limit}`;

    const res = await fetch(url, { credentials: "include" });

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

async function loadBuyerAds() {
  buyerLoading.value = true;
  buyerError.value = "";

  try {
    const res = await fetch(`/api/BuyerAd/all/${props.limit}?t=${Date.now()}`, {
      credentials: "include",
      cache: "no-store",
    });

    if (res.status === 401 || res.status === 403) {
      buyerAds.value = [];
      buyerError.value = "";
      return;
    }

    if (!res.ok) {
      const text = await res.text().catch(() => "");
      buyerAds.value = [];
      buyerError.value = `Nie udało się pobrać zleceń (${res.status}). ${String(text).slice(0, 120)}`;
      return;
    }

    const data = await res.json();
    buyerAds.value = Array.isArray(data) ? data : [];
  } catch {
    buyerAds.value = [];
    buyerError.value = "Błąd sieci przy pobieraniu zleceń klientów.";
  } finally {
    buyerLoading.value = false;
  }
}

function onSearchChanged(e) {
  searchTerm.value = e?.detail ?? "";
  loadOffers();
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

  window.addEventListener("search:changed", onSearchChanged);
  window.addEventListener("sellerad:created", onSellerCreated);
  window.addEventListener("buyerad:created", onBuyerCreated);
});

onUnmounted(() => {
  window.removeEventListener("search:changed", onSearchChanged);
  window.removeEventListener("sellerad:created", onSellerCreated);
  window.removeEventListener("buyerad:created", onBuyerCreated);
});
</script>

<template>
  <section class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 sm:py-16">
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
    
    <div v-else-if="filteredOffers.length === 0" class="bg-zinc-50 dark:bg-zinc-900/50 p-12 text-center rounded-2xl border border-zinc-200 dark:border-zinc-800">
      <p class="text-zinc-500 dark:text-zinc-400 text-lg">Brak dostępnych ofert w tym momencie.</p>
    </div>

    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      <RouterLink
        v-for="o in filteredOffers"
        :key="o.id"
        :to="`/offer/${o.id}`"
        class="block h-full"
      >
        <OfferCard :offer="o" />
      </RouterLink>
    </div>

    <div class="flex items-center justify-between mb-8 mt-16">
      <h2 class="text-2xl sm:text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">
        Zlecenia klientów
      </h2>
    </div>

    <div v-if="buyerLoading" class="flex justify-center items-center py-20">
      <div class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-teal-600"></div>
    </div>
    
    <div v-else-if="buyerError" class="bg-red-50 dark:bg-red-900/20 text-red-600 dark:text-red-400 p-6 rounded-2xl text-center border border-red-100 dark:border-red-900/30">
      <p class="font-medium">{{ buyerError }}</p>
    </div>
    
    <div v-else-if="filteredBuyerAds.length === 0" class="bg-zinc-50 dark:bg-zinc-900/50 p-12 text-center rounded-2xl border border-zinc-200 dark:border-zinc-800">
      <p class="text-zinc-500 dark:text-zinc-400 text-lg">Brak dostępnych zleceń w tym momencie.</p>
    </div>

    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      <RouterLink
        v-for="o in filteredBuyerAds"
        :key="o.id"
        :to="`/request/${o.id}`"
        class="block h-full"
      >
        <OfferCard :offer="o" />
      </RouterLink>
    </div>
  </section>
</template>
