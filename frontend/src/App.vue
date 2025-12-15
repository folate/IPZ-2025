<script setup>
import LandingHeader from "./components/landing/LandingHeader.vue"
import LandingSearch from "./components/landing/LandingSearch.vue"
import CategoriesRow from "./components/landing/CategoriesRow.vue"
import OffersGrid from "./components/landing/OffersGrid.vue"
</script>

<template>
  <div class="page">
    <LandingHeader />
    <LandingSearch />
    <CategoriesRow />
    <OffersGrid />
  </div>
</template>

<style scoped>
.page
{
  min-height: 100vh;
  background: #f0f0f0;
  color: #111;
}
</style>
