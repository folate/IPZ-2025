<script setup>
import LandingHeader from "@/components/landing/LandingHeader.vue";
import * as vue from "vue";
import { useRoute } from "vue-router";
import offerImage from "../../../public/Placeholders/offerImage.png";
const route = useRoute();
const offerDetails = vue.ref(null);
const error = vue.ref("");
const isFav = vue.ref(false);

vue.onMounted(() => {
  fetchDetails();
  checkFavs();
});

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

function checkFavs() {
  const item = localStorage.getItem("FavoritesIds");
  const favorites = item ? JSON.parse(item) : [];
  isFav.value = favorites.includes(route.params.id);
}

function buyTier(tier) {
  console.log("tier bought:", tier.tierName, " ", tier.price, "zł");
}

function addFavorites() {
  const id = route.params.id;
  const item = localStorage.getItem("FavoritesIds");
  let favorites = item ? JSON.parse(item) : [];

  const index = favorites.indexOf(id);
  if (index === -1) {
    favorites.push(id);
    isFav.value = true;
  } else {
    favorites.splice(index, 1);
    isFav.value = false;
  }
  localStorage.setItem("FavoritesIds", JSON.stringify(favorites));
}
</script>
<template>
  <main>
    <LandingHeader />
    <div class="detailsPage">
      <div v-if="offerDetails">
        <p class="Title">{{ offerDetails.title }}</p>
        <button
          v-on:click="addFavorites()"
          id="Favorites"
          :class="isFav ? 'Fav' : 'NotFav'"
        >
          Favorite!
        </button>
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
            <button v-on:click="buyTier(tier)">Buy!</button>
          </div>
        </div>
      </div>

      <p v-if="error" class="offersError">{{ error }}</p>
    </div>
  </main>
</template>
