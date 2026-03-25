<script setup>
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
} from "@/components/ui/dropdown-menu"
import { Menu, ChevronDown } from "lucide-vue-next"

import { ref, computed, onMounted } from "vue"
import { useRoute, useRouter } from "vue-router"

const route = useRoute()
const router = useRouter()
const categories = ref([])

onMounted(async () => {
  try {
    const res = await fetch("/api/Category/top")
    if (res.ok) {
      let data = await res.json()
      if (Array.isArray(data)) {
        data = data.sort((a, b) => (b.adsCount || 0) - (a.adsCount || 0))
      }
      categories.value = data.map(c => c.name || c.Name)
    }
  } catch (err) {
    console.error("Failed to fetch categories:", err)
  }
})

const topCats = computed(() => categories.value.slice(0, 5))
const allCats = computed(() => categories.value.slice(5))

function pick(c) {
  router.push({ path: '/search', query: { ...route.query, category: c } })
}
</script>

<template>
  <section class="w-full bg-zinc-50 dark:bg-zinc-900/50 py-6 border-b border-zinc-200 dark:border-zinc-800">
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex flex-wrap justify-center items-center gap-3 sm:gap-4 overflow-x-auto pb-2 sm:pb-0 scrollbar-hide">

        <button
          v-for="c in topCats"
          :key="c"
          class="flex items-center whitespace-nowrap rounded-xl bg-white dark:bg-zinc-950 border border-zinc-200 dark:border-zinc-800 px-5 py-2.5 text-sm font-medium text-zinc-700 dark:text-zinc-300 hover:border-teal-500 hover:text-teal-600 dark:hover:text-teal-400 hover:shadow-sm transition-all focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500"
          type="button"
          @click="pick(c)"
        >
          {{ c }}
        </button>

        <DropdownMenu v-if="allCats.length > 0">
          <DropdownMenuTrigger asChild>
            <button class="flex items-center gap-2 whitespace-nowrap rounded-xl bg-teal-600 hover:bg-teal-700 text-white px-5 py-2.5 text-sm font-semibold transition-colors shadow-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500">
              <Menu class="w-5 h-5" />
              Pozostałe Kategorie
              <ChevronDown class="w-4 h-4 ml-1 opacity-70" />
            </button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="start" class="w-56 rounded-xl z-50">
            <DropdownMenuItem 
              v-for="c in allCats" 
              :key="c" 
              @click="pick(c)"
              class="cursor-pointer"
            >
              {{ c }}
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>

      </div>
    </div>
  </section>
</template>

