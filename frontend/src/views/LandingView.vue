<script setup>
import LandingHeader from "../components/landing/LandingHeader.vue"
import LandingSearch from "../components/landing/LandingSearch.vue"
import CategoriesRow from "../components/landing/CategoriesRow.vue"
import OffersGrid from "../components/landing/OffersGrid.vue"
</script>

<template>
  <main>
    <LandingHeader />
    <LandingSearch />
    <CategoriesRow />
    <OffersGrid />
  </main>
</template>

<style scoped>
main {
  display: flex;
  flex-direction: column;
  gap: 24px;
}
</style>