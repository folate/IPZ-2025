<script setup>
import { ref, watch } from "vue";
import { useRouter, useRoute } from "vue-router";
import { Input } from "@/components/ui/input";
import { Search, ListFilter } from "lucide-vue-next";
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
} from "@/components/ui/dropdown-menu"
import CategoriesRow from "@/components/landing/CategoriesRow.vue";
import OffersGrid from "@/components/landing/OffersGrid.vue";
import Container from "@/components/ui/Container.vue";

const router = useRouter();
const route = useRoute();
const query = ref(route.query.q || "");

watch(() => route.query.q, (newQ) => {
  query.value = newQ || "";
});

function onSubmit(e) {
  e.preventDefault();
  router.push({ 
    path: '/search', 
    query: { ...route.query, q: query.value.trim() || undefined } 
  });
}

function setSort(sortBy, order) {
  router.push({
    path: '/search',
    query: { ...route.query, sortBy, order }
  });
}
</script>

<template>
  <div class="flex flex-col w-full">
    
    <div class="w-full bg-zinc-50 dark:bg-zinc-900 border-b border-zinc-200 dark:border-zinc-800 py-6 animate-in slide-in-from-top duration-500">
      <Container class="flex flex-col md:flex-row justify-center items-center gap-4">
        <form class="relative w-full max-w-2xl group" @submit="onSubmit">
          <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none text-zinc-400 group-focus-within:text-teal-600 transition-colors z-10">
            <Search class="h-5 w-5" />
          </div>
          <Input 
            v-model="query"
            class="h-12 w-full pl-11 pr-24 rounded-xl border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-950 shadow-sm text-base focus-visible:ring-teal-500 focus-visible:ring-offset-0 transition-all"
            placeholder="Znajdź usługi..." 
          />
          <button type="submit" class="absolute right-1.5 top-1.5 bottom-1.5 bg-teal-600 hover:bg-teal-700 text-white px-4 rounded-lg font-medium transition-colors shadow-sm cursor-pointer z-10 text-sm">
            Szukaj
          </button>
        </form>

        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <button class="flex items-center gap-2 whitespace-nowrap rounded-xl bg-white dark:bg-zinc-950 border border-zinc-200 dark:border-zinc-800 px-5 py-3 text-sm font-medium text-zinc-700 dark:text-zinc-300 hover:border-teal-500 hover:text-teal-600 transition-all shadow-sm">
              <ListFilter class="w-4 h-4" />
              Sortowanie
            </button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" class="w-56 rounded-xl">
            <DropdownMenuLabel>Sortuj według</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem @click="setSort('name', 'asc')" class="cursor-pointer">Nazwa (A-Z)</DropdownMenuItem>
            <DropdownMenuItem @click="setSort('name', 'desc')" class="cursor-pointer">Nazwa (Z-A)</DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem @click="setSort('price', 'asc')" class="cursor-pointer">Cena (rosnąco)</DropdownMenuItem>
            <DropdownMenuItem @click="setSort('price', 'desc')" class="cursor-pointer">Cena (malejąco)</DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </Container>
    </div>

    <!-- Filtry Kategorii -->
    <CategoriesRow />
    
    <div class="flex-1 w-full bg-zinc-50 dark:bg-zinc-950 animate-in fade-in duration-700">
      <OffersGrid :limit="50" />
    </div>
  </div>
</template>
