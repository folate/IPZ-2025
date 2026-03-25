<script setup>
import { RouterLink } from 'vue-router'
import LandingSearch from "../components/landing/LandingSearch.vue"
import CategoriesRow from "../components/landing/CategoriesRow.vue"
import OffersGrid from "../components/landing/OffersGrid.vue"
import { Button } from '@/components/ui/button'
</script>

<template>
  <main class="flex flex-col w-full">
    <LandingSearch />
    <CategoriesRow />
    <OffersGrid />
    
    <!-- Become a Seller CTA Section -->
    <section class="w-full bg-primary/5 py-16 mt-12 border-t">
      <div class="container mx-auto px-4 flex flex-col items-center text-center space-y-6">
        <h2 class="text-3xl font-bold tracking-tight text-foreground">
          Masz umiejętności? Zacznij zarabiać już dziś!
        </h2>
        <p class="text-muted-foreground text-lg max-w-[600px]">
          Dołącz do naszej społeczności utalentowanych freelancerów. Oferuj swoje usługi, zdobywaj klientów i rozwijaj swój biznes na własnych zasadach.
        </p>
        <RouterLink to="/register">
          <Button size="lg" class="mt-4 text-base px-8">
            Zostań freelancerem
          </Button>
        </RouterLink>
      </div>
    </section>
  </main>
</template>