<script setup>
import { useRouter } from "vue-router";

const router = useRouter();
const currentYear = new Date().getFullYear();
</script>

<template>
  <footer class="w-full border-t bg-background/80 backdrop-blur-md mt-auto">
    <div class="w-full max-w-7xl mx-auto px-4 py-12 sm:px-6 lg:px-8">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
        <!-- Logo and About -->
        <div class="col-span-1 md:col-span-2">
          <div class="text-2xl font-black tracking-tighter text-teal-600 dark:text-teal-500 cursor-pointer mb-4" @click="router.push('/')">
            LOGO
          </div>
          <p class="text-zinc-600 dark:text-zinc-400 text-sm max-w-xs">
            Twoja platforma do znajdowania i oferowania profesjonalnych usług. Łączymy talenty z potrzebami.
          </p>
        </div>

        <!-- Quick Links -->
        <div>
          <h3 class="text-sm font-semibold text-zinc-900 dark:text-zinc-100 uppercase tracking-wider mb-4">Platforma</h3>
          <ul class="space-y-2">
            <li>
              <router-link to="/" class="text-zinc-600 dark:text-zinc-400 hover:text-teal-600 dark:hover:text-teal-400 text-sm transition-colors">Strona Główna</router-link>
            </li>
            <li>
              <router-link to="/categories" class="text-zinc-600 dark:text-zinc-400 hover:text-teal-600 dark:hover:text-teal-400 text-sm transition-colors">Kategorie</router-link>
            </li>
            <li>
              <router-link to="/offers" class="text-zinc-600 dark:text-zinc-400 hover:text-teal-600 dark:hover:text-teal-400 text-sm transition-colors">Oferty</router-link>
            </li>
          </ul>
        </div>

        <!-- Support/Legal -->
        <div>
          <h3 class="text-sm font-semibold text-zinc-900 dark:text-zinc-100 uppercase tracking-wider mb-4">Pomoc</h3>
          <ul class="space-y-2">
            <li>
              <router-link to="/faq" class="text-zinc-600 dark:text-zinc-400 hover:text-teal-600 dark:hover:text-teal-400 text-sm transition-colors">FAQ</router-link>
            </li>
            <li>
              <router-link to="/contact" class="text-zinc-600 dark:text-zinc-400 hover:text-teal-600 dark:hover:text-teal-400 text-sm transition-colors">Kontakt</router-link>
            </li>
            <li>
              <router-link to="/privacy" class="text-zinc-600 dark:text-zinc-400 hover:text-teal-600 dark:hover:text-teal-400 text-sm transition-colors">Prywatność</router-link>
            </li>
          </ul>
        </div>
      </div>

      <div class="mt-12 border-t pt-8">
        <p class="text-center text-zinc-500 dark:text-zinc-500 text-xs">
          © {{ currentYear }} LOGO Sp. z o.o. Wszystkie prawa zastrzeżone.
        </p>
      </div>
    </div>
  </footer>
</template>
