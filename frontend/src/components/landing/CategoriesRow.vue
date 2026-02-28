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

const topCats = ["KAT A", "KAT B", "KAT C", "KAT D", "KAT E"]
const allCats = ["KAT1", "KAT2", "KAT3", "KAT4", "KAT5", "ADVANCED..."]

function pick(c) {
  // placeholder function
  console.log("Wybrano kategorię:", c)
}
</script>

<template>
  <section class="w-full bg-zinc-50 dark:bg-zinc-900/50 py-6 border-b border-zinc-200 dark:border-zinc-800">
    <div class="container mx-auto px-4 sm:px-6">
      <div class="flex flex-wrap items-center gap-3 sm:gap-4 overflow-x-auto pb-2 sm:pb-0 scrollbar-hide">
        
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <button class="flex items-center gap-2 whitespace-nowrap rounded-xl bg-teal-600 hover:bg-teal-700 text-white px-5 py-2.5 text-sm font-semibold transition-colors shadow-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500">
              <Menu class="w-5 h-5" />
              Kategorie
              <ChevronDown class="w-4 h-4 ml-1 opacity-70" />
            </button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="start" class="w-56 rounded-xl z-50">
            <DropdownMenuLabel>Wszystkie kategorie</DropdownMenuLabel>
            <DropdownMenuSeparator />
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

        <button
          v-for="c in topCats"
          :key="c"
          class="flex items-center whitespace-nowrap rounded-xl bg-white dark:bg-zinc-950 border border-zinc-200 dark:border-zinc-800 px-5 py-2.5 text-sm font-medium text-zinc-700 dark:text-zinc-300 hover:border-teal-500 hover:text-teal-600 dark:hover:text-teal-400 hover:shadow-sm transition-all focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500"
          type="button"
          @click="pick(c)"
        >
          {{ c }}
        </button>

      </div>
    </div>
  </section>
</template>

