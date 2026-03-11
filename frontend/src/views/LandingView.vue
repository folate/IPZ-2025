<script setup>
import LandingHeader from "../components/landing/LandingHeader.vue"
import LandingSearch from "../components/landing/LandingSearch.vue"
import CategoriesRow from "../components/landing/CategoriesRow.vue"
import OffersGrid from "../components/landing/OffersGrid.vue"
</script>

<template>
  <main class="min-h-screen bg-background flex flex-col w-full">
    <LandingHeader />
    <LandingSearch />
    <CategoriesRow />
    <OffersGrid />
  </main>
</template>