<script setup>
import LandingHeader from "@/components/landing/LandingHeader.vue";
import * as vue from "vue";
const offers = vue.ref([]);
const error = vue.ref("");
vue.onMounted(fetchDetails);
async function fetchDetails() {
  const item = localStorage.getItem("FavoritesIds");
  const favorites = JSON.parse(item);
  if (favorites.length > 0) {
    for (const id in favorites) {
      try {
        const res = await fetch(`/api/SellerAd/${id}`);
        if (!res.ok) {
          throw new Error(`Błąd pobierania szczegółów (${res.status})`);
        }
        const data = await res.json();
        offers.value.push(data);
      } catch (err) {
        error.value = err.message;
      }
    }
  }
}
</script>
<template>
  <main>
    <LandingHeader />
    <div class="offersGrid">
      <RouterLink v-for="o in offers" :key="o.id" :to="`/offer/${o.id}`">
        <OfferCard :offer="o" />
      </RouterLink>
    </div>
  </main>
</template>
