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
  <section class="offersSection">
    <h2 class="offersTitle">Oferty</h2>

    <p v-if="loading" class="offersInfo">Ładowanie...</p>
    <p v-else-if="error" class="offersError">{{ error }}</p>
    <p v-else-if="filteredOffers.length === 0" class="offersInfo">
      Brak ofert.
    </p>

    <div class="offersGrid">
      <RouterLink
        v-for="o in filteredOffers"
        :key="o.id"
        :to="`/offer/${o.id}`"
      >
        <OfferCard :offer="o" />
      </RouterLink>
    </div>

    <h2 class="offersTitle" style="margin-top: 50px">Zlecenia klientów</h2>

    <p v-if="buyerLoading" class="offersInfo">Ładowanie...</p>
    <p v-else-if="buyerError" class="offersError">{{ buyerError }}</p>
    <p v-else-if="filteredBuyerAds.length === 0" class="offersInfo">
      Brak zleceń.
    </p>

    <div class="offersGrid">
      <RouterLink
        v-for="o in filteredBuyerAds"
        :key="o.id"
        :to="`/request/${o.id}`"
      >
        <OfferCard :offer="o" />
      </RouterLink>
    </div>
  </section>
</template>
