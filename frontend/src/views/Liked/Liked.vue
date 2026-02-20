<script setup>
import LandingHeader from "@/components/landing/LandingHeader.vue";
import * as vue from "vue";
import { useRoute } from "vue-router";
const route = useRoute();
const Liked = vue.ref(null);
const error = vue.ref("");
vue.onMounted(fetchDetails);
async function fetchDetails() {
  //   try {
  //     const res = await fetch(`/api/SellerAd/${id}`);
  //     if (!res.ok) {
  //       throw new Error(`Błąd pobierania szczegółów (${res.status})`);
  //     }
  //     offerDetails.value = await res.json();
  //   } catch (err) {
  //     error.value = err.message;
  //   }
}
</script>
<template>
  <main>
    <LandingHeader />
    LIKED OFFERS
  </main>
</template>
