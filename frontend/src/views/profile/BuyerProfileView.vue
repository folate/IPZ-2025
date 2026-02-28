<script setup>
import LandingHeader from "@/components/landing/LandingHeader.vue"
import { Card, CardContent } from "@/components/ui/card"
</script>

<template>
  <div class="min-h-svh bg-zinc-50 dark:bg-zinc-950 flex flex-col">
    <LandingHeader />
    <div class="flex-1 flex items-center justify-center p-6">
      <Card class="w-full max-w-lg shadow-xl shadow-teal-900/5 border-zinc-200 dark:border-zinc-800">
        <CardContent class="p-10 flex flex-col items-center justify-center text-center gap-4">
          <h1 class="text-3xl font-extrabold text-zinc-900 dark:text-zinc-50">Buyer Profile</h1>
          <p class="text-zinc-500 dark:text-zinc-400 text-lg">
            TODO – handled in I2-113
          </p>
        </CardContent>
      </Card>
    </div>
  </div>
</template>