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

const searchTerm = ref("");
const category = ref("");

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

  try {
    const q = norm(searchTerm.value);
    const cat = category.value;

    let url = `/api/SellerAd/all/${props.limit}`;

    if (q || cat) {
      url = `/api/Search?search=${encodeURIComponent(q)}&category=${encodeURIComponent(cat)}`;
    }

    const res = await fetch(url, { credentials: "include" });

    if (!res.ok) {
      offers.value = [];
      return;
    }

    const data = await res.json();
    offers.value = Array.isArray(data) ? data : [];
  } catch {
    offers.value = [];
  } finally {
    loading.value = false;
  }
}

async function loadBuyerAds() {
  buyerLoading.value = true;

  try {
    const res = await fetch(`/api/BuyerAd/all/${props.limit}?t=${Date.now()}`, {
      credentials: "include",
      cache: "no-store",
    });

    if (!res.ok) {
      buyerAds.value = [];
      return;
    }

    const data = await res.json();
    buyerAds.value = Array.isArray(data) ? data : [];
  } catch {
    buyerAds.value = [];
  } finally {
    buyerLoading.value = false;
  }
}

function onSearchChanged(e) {
  searchTerm.value = e?.detail ?? "";
  loadOffers();
}

function onCategoryChanged(e) {
  category.value = e.detail ?? "";
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
  window.addEventListener("category:changed", onCategoryChanged);
  window.addEventListener("sellerad:created", onSellerCreated);
  window.addEventListener("buyerad:created", onBuyerCreated);
});

onUnmounted(() => {
  window.removeEventListener("search:changed", onSearchChanged);
  window.removeEventListener("sellerad:created", onSellerCreated);
  window.removeEventListener("buyerad:created", onBuyerCreated);
  window.removeEventListener("category:changed", onCategoryChanged);
});
</script>

<template>
  <section class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 sm:py-16">
    <div class="flex items-center justify-between mb-8">
      <h2
        class="text-2xl sm:text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50"
      >
        Polecane Oferty
      </h2>
    </div>

    <div v-if="loading" class="flex justify-center items-center py-20">
      <div
        class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-teal-600"
      ></div>
    </div>

    <div
      v-else-if="filteredOffers.length === 0"
      class="bg-zinc-50 dark:bg-zinc-900/50 p-12 text-center rounded-2xl border border-zinc-200 dark:border-zinc-800"
    >
      <p class="text-zinc-500 dark:text-zinc-400 text-lg">Brak ofert.</p>
    </div>

    <div
      v-else
      class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6"
    >
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
      <h2
        class="text-2xl sm:text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50"
      >
        Zlecenia klientów
      </h2>
    </div>

    <div v-if="buyerLoading" class="flex justify-center items-center py-20">
      <div
        class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-teal-600"
      ></div>
    </div>

    <div
      v-else-if="filteredBuyerAds.length === 0"
      class="bg-zinc-50 dark:bg-zinc-900/50 p-12 text-center rounded-2xl border border-zinc-200 dark:border-zinc-800"
    >
      <p class="text-zinc-500 dark:text-zinc-400 text-lg">Brak ofert.</p>
    </div>

    <div
      v-else
      class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6"
    >
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
