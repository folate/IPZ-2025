<script setup>
import { Input } from "@/components/ui/input"
import { Search } from "lucide-vue-next"
import { ref, onBeforeUnmount } from "vue";

const query = ref("");
let t = null;

function emitSearch(value) {
  window.dispatchEvent(new CustomEvent("search:changed", { detail: value }));
}

function onInput(e) {
  query.value = e.target.value;

  if (t) clearTimeout(t);
  t = setTimeout(() => {
    emitSearch(query.value);
  }, 300);
}

function onSubmit(e) {
  e.preventDefault();
  emitSearch(query.value);
}

onBeforeUnmount(() => {
  if (t) clearTimeout(t);
});
</script>

<template>
  <section class="w-full bg-gradient-to-b from-teal-50/50 to-background dark:from-teal-950/20 py-16 sm:py-24">
    <div class="container mx-auto px-4 sm:px-6 flex flex-col items-center justify-center text-center space-y-8">
      <div class="space-y-4 max-w-3xl">
        <h1 class="text-4xl sm:text-5xl md:text-6xl font-black tracking-tight text-zinc-900 dark:text-zinc-50">
          Znajdź to, czego <span class="text-transparent bg-clip-text bg-gradient-to-r from-teal-500 to-teal-700">potrzebujesz.</span>
        </h1>
        <p class="text-lg sm:text-xl text-zinc-500 dark:text-zinc-400 max-w-2xl mx-auto">
          Odkryj najlepszych freelancerów i wyjątkowe usługi dla Twojego kolejnego projektu.
        </p>
      </div>
      
      <form class="relative w-full max-w-3xl mx-auto group" @submit="onSubmit">
        <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none text-zinc-400 group-focus-within:text-teal-600 transition-colors z-10">
          <Search class="h-6 w-6" />
        </div>
        <Input 
          :model-value="query"
          @input="onInput"
          class="h-16 w-full pl-12 pr-[120px] rounded-2xl border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 shadow-sm text-lg focus-visible:ring-teal-500 focus-visible:ring-offset-0 transition-all hover:shadow-md"
          placeholder="Jakiej usługi dzisiaj szukasz?" 
        />
        <button type="submit" class="absolute right-2 top-2 bottom-2 bg-teal-600 hover:bg-teal-700 dark:bg-teal-500 dark:hover:bg-teal-600 text-white px-6 rounded-xl font-medium transition-colors shadow-sm cursor-pointer z-10">
          Szukaj
        </button>
      </form>
    </div>
  </section>
</template>
