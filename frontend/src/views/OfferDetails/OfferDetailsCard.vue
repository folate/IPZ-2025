<script setup>
import LandingHeader from "@/components/landing/LandingHeader.vue";
import * as vue from "vue";
import { useRoute } from "vue-router";
const route = useRoute();
const offerDetails = vue.ref(null);
const error = vue.ref("");

async function fetchDetails() {
  const id = route.params.id;
  try {
    const res = await fetch(`/api/SellerAd/${id}`);
    if (!res.ok) {
      throw new Error(`Błąd pobierania szczegółów (${res.status})`);
    }
    offerDetails.value = await res.json();
  } catch (err) {
    error.value = err.message;
  }
}

vue.onMounted(fetchDetails);

function buyTier() {
  console.log("tier bought");
}
</script>
<template>
  <main>
    <LandingHeader />
    <div class="detailsPage">
      <div v-if="offerDetails">
        <p class="Title">{{ offerDetails.title }}</p>
        <img :src="offerImage" alt="Offer Image" />
        <p class="Description">{{ offerDetails.description }}</p>
        <p class="Username">{{ offerDetails.freelancer }}</p>
        <div class="Tiers">
          <div
            class="Tier"
            v-for="tier in offerDetails.gigs"
            :key="tier.tierName"
          >
            <p class="Title">{{ tier.tierName }}</p>
            <p class="Description">{{ tier.tierDescription }}</p>
            <p class="Price">{{ tier.price }}</p>
            <button v-on:click="buyTier()">Buy!</button>
          </div>
        </div>
      </div>

      <p v-if="error" class="offersError">{{ error }}</p>
    </div>
  </main>
</template>
