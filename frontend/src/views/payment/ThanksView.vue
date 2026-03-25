<script setup>
import { useRouter } from "vue-router";
import Container from "@/components/ui/Container.vue";
import { Button } from "@/components/ui/button";
import { CheckCircle } from "lucide-vue-next";

const router = useRouter();
</script>

<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20">
    <Container class="flex-1 flex flex-col items-center justify-center py-20">
      <div class="flex flex-col items-center text-center max-w-lg">
        <div class="w-24 h-24 bg-teal-100 dark:bg-teal-900/30 rounded-full flex items-center justify-center mb-8">
          <CheckCircle class="w-12 h-12 text-teal-600 dark:text-teal-400" />
        </div>
        <h1 class="text-4xl font-extrabold text-zinc-900 dark:text-zinc-50 mb-4">
          Dziękujemy za zamówienie!
        </h1>
        <p class="text-lg text-zinc-600 dark:text-zinc-400 mb-8">
          Twoja płatność została przetworzona pomyślnie. Wykonawca został poinformowany o nowym zleceniu. Możesz śledzić status zamówienia w swoim panelu użytkownika.
        </p>
        <div class="flex gap-4">
          <Button @click="router.push('/buyer/profile')" class="bg-teal-600 hover:bg-teal-700 text-white font-semibold">
            Przejdź do profilu
          </Button>
          <Button variant="outline" @click="router.push('/')">
            Wróć na stronę główną
          </Button>
        </div>
      </div>
    </Container>
  </div>
</template>
