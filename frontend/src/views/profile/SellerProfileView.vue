<script setup>
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { Search, Heart, MessageCircle, Star, TriangleAlert } from "lucide-vue-next";

import LandingHeader from "../../components/landing/LandingHeader.vue";
import Container from "../../components/ui/Container.vue";
import OfferCard from "@/components/landing/OfferCard.vue";

import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

const router = useRouter();

const activeTab = ref("offers");
const loading = ref(false);
const error = ref("");

const tabs = [
  { key: "offers", label: "Oferty" },
  { key: "reviews", label: "Opinie" },
  { key: "description", label: "O mnie" },
];

const me = ref(null);
const offers = ref([]);
const categories = ref([]);
const selectedCategory = ref("all");
const searchText = ref("");

const displayName = computed(() => me.value?.login ?? "SELLER");

function offerOwnerLogin(o) {
  return (
    o?.freelancer ??
    o?.freelancerLogin ??
    o?.seller ??
    o?.sellerLogin ??
    o?.login ??
    null
  );
}

const filteredOffers = computed(() => {
  const q = searchText.value.trim().toLowerCase();
  const cat = selectedCategory.value;

  return (offers.value || []).filter((o) => {
    if (cat !== "all") {
      const offerCat = String(o?.category ?? "").trim();
      if (offerCat !== cat) return false;
    }

    if (!q) return true;

    const title = String(o?.title ?? "").toLowerCase();
    const desc = String(o?.description ?? "").toLowerCase();

    return title.includes(q) || desc.includes(q);
  });
});

async function load() {
  loading.value = true;
  error.value = "";
  me.value = null;
  offers.value = [];

  try {
    const resUser = await fetch("/api/User/getUser", {
      credentials: "include",
    });

    if (!resUser.ok) {
      error.value = "Nie jesteś zalogowany.";
      return;
    }

    const userData = await resUser.json();
    me.value = userData;

    const login = userData?.login;
    if (!login) {
      error.value = "Brak loginu w getUser.";
      return;
    }

    const resOffers = await fetch("/api/sellerad/all/200", {
      credentials: "include",
    });

    if (!resOffers.ok) {
      error.value = `Nie udało się pobrać ofert (${resOffers.status}).`;
      return;
    }

    const all = await resOffers.json();
    const list = Array.isArray(all) ? all : [];

    offers.value = list.filter((o) => offerOwnerLogin(o) === login);
  } catch (e) {
    error.value = e?.message ?? "Błąd podczas pobierania profilu.";
  } finally {
    loading.value = false;
  }
}

async function loadCategories() {
  try {
    const res = await fetch("/api/category", {
      credentials: "include",
    });

    if (!res.ok) return;

    const data = await res.json();
    categories.value = Array.isArray(data) ? data : [];
  } catch {
    categories.value = [];
  }
}

onMounted(async () => {
  await Promise.all([load(), loadCategories()]);
});

function selectCat(name) {
  selectedCategory.value = name;
}

const isFavourite = ref(false);

function onToggleFavourite() {
  isFavourite.value = !isFavourite.value;
}

function onChatClick() {
  console.log("chat click");
}

function onReport() {
  alert("Reported user");
}

// mocks for reviews
const reviewStats = {
  total: 123,
  avg: 4.2,
  stars: [
    { stars: 5, count: 47 },
    { stars: 4, count: 35 },
    { stars: 3, count: 20 },
    { stars: 2, count: 9 },
    { stars: 1, count: 12 },
  ],
};

const reviews = [
  { id: 1, user: "Kasia", stars: 5, text: "Świetna robota, szybki czas realizacji!" },
  { id: 2, user: "Tomek", stars: 4, text: "Dobry kontakt, ale musiałem poprosić o poprawki." },
  { id: 3, user: "Michał", stars: 3, text: "Projektowanie mogłoby być trochę bardziej kreatywne." },
];
</script>

<template>
  <div class="min-h-svh bg-zinc-50 dark:bg-zinc-950 pb-20">
    <LandingHeader />

    <!-- Banner -->
    <div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 mt-6">
      <div class="h-48 md:h-64 w-full bg-zinc-800 dark:bg-zinc-900 rounded-2xl relative overflow-hidden flex items-center shadow-lg">
        <div class="absolute inset-0 bg-teal-900/40 mix-blend-multiply"></div>
        <img src="https://images.unsplash.com/photo-1542831371-29b0f74f9713?q=80&w=2000&auto=format&fit=crop" class="absolute inset-0 w-full h-full object-cover opacity-60 mix-blend-overlay" />
        <h1 class="absolute left-8 md:left-12 text-6xl md:text-8xl font-black text-white/20 tracking-widest pointer-events-none uppercase">Banner</h1>
      </div>
    </div>

    <Container>
      <!-- Top Meta Row -->
      <div class="mt-8 flex flex-col md:flex-row md:items-center justify-between gap-6">
        
        <!-- Avatar & Name -->
        <div class="flex items-center gap-5">
          <Avatar class="h-20 w-20 md:h-24 md:w-24 border-4 border-zinc-100 dark:border-zinc-900 shadow-xl bg-teal-50 dark:bg-teal-900/20">
            <AvatarImage src="" alt="Seller Avatar" />
            <AvatarFallback class="text-3xl text-teal-700 dark:text-teal-400 font-bold bg-zinc-100 dark:bg-zinc-800">
              {{ displayName.charAt(0).toUpperCase() }}
            </AvatarFallback>
          </Avatar>
          
          <div class="flex flex-col">
            <h1 class="text-2xl md:text-3xl font-extrabold text-zinc-900 dark:text-zinc-50">
              {{ displayName }}
            </h1>
            <div class="flex items-center gap-1 mt-1 text-teal-600 dark:text-teal-400">
              <Star v-for="i in 5" :key="i" class="h-4 w-4 md:h-5 md:w-5" :class="i <= Math.round(reviewStats.avg) ? 'fill-current' : 'text-zinc-300 dark:text-zinc-700'" />
            </div>
          </div>
        </div>

        <!-- Actions & Search -->
        <div class="flex items-center gap-3">
          <div class="relative w-full max-w-xs md:w-64">
            <Search class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-zinc-400" />
            <Input 
              v-model="searchText" 
              placeholder="Szukaj w ofertach..." 
              class="pl-9 bg-white dark:bg-zinc-900 border-zinc-200 dark:border-zinc-800 focus-visible:ring-teal-500 rounded-xl h-11"
            />
          </div>

          <Button variant="outline" size="icon" @click="onToggleFavourite" 
                  class="h-11 w-11 rounded-xl border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 hover:bg-zinc-100 dark:hover:bg-zinc-800 transition-colors shrink-0">
            <Heart class="h-5 w-5 transition-colors" :class="isFavourite ? 'fill-rose-500 text-rose-500' : 'text-zinc-500 dark:text-zinc-400'" />
          </Button>

          <Button variant="outline" size="icon" @click="onChatClick" 
                  class="h-11 w-11 rounded-xl border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 hover:bg-zinc-100 dark:hover:bg-zinc-800 transition-colors shrink-0">
            <MessageCircle class="h-5 w-5 text-zinc-500 dark:text-zinc-400" />
          </Button>
        </div>
      </div>

      <!-- Main Content / Tabs -->
      <div class="mt-8">
        <Tabs defaultValue="offers" v-model="activeTab" class="w-full">
          <TabsList class="w-full justify-start h-auto bg-transparent border-b border-zinc-200 dark:border-zinc-800 p-0 rounded-none gap-8">
            <TabsTrigger 
              v-for="t in tabs" 
              :key="t.key" 
              :value="t.key"
              class="rounded-none border-b-2 border-transparent px-0 py-3 text-lg font-semibold text-zinc-500 dark:text-zinc-400 data-[state=active]:border-teal-600 data-[state=active]:text-teal-700 dark:data-[state=active]:text-teal-400 data-[state=active]:shadow-none data-[state=active]:bg-transparent focus-visible:ring-0"
            >
              {{ t.label }}
            </TabsTrigger>
          </TabsList>

          <!-- Description Tab -->
          <TabsContent value="description" class="pt-8 flex flex-col gap-2">
             <p class="text-xl md:text-2xl font-medium text-zinc-600 dark:text-zinc-300">
               Tu będzie opis profilu.
             </p>
          </TabsContent>

          <!-- Offers Tab -->
          <TabsContent value="offers" class="pt-8">
            <div class="flex flex-col md:flex-row gap-8">
              
              <!-- Left Sidebar -->
              <aside class="w-full md:w-56 flex flex-col gap-1 md:border-r border-zinc-200 dark:border-zinc-800 md:pr-6 shrink-0">
                <button
                  class="text-left py-2 px-3 rounded-lg text-lg font-semibold transition-colors"
                  :class="selectedCategory === 'all' ? 'bg-zinc-100 dark:bg-zinc-800 text-zinc-900 dark:text-zinc-100' : 'text-zinc-500 dark:text-zinc-400 hover:text-zinc-900 dark:hover:text-zinc-100 hover:bg-zinc-50 dark:hover:bg-zinc-900/50'"
                  @click="selectCat('all')"
                >
                  Wszystko
                </button>

                <button
                  v-for="c in categories"
                  :key="c.name"
                  class="text-left py-2 px-3 rounded-lg text-lg font-semibold transition-colors"
                  :class="selectedCategory === c.name ? 'bg-zinc-100 dark:bg-zinc-800 text-zinc-900 dark:text-zinc-100' : 'text-zinc-500 dark:text-zinc-400 hover:text-zinc-900 dark:hover:text-zinc-100 hover:bg-zinc-50 dark:hover:bg-zinc-900/50'"
                  @click="selectCat(c.name)"
                >
                  {{ c.name }}
                </button>

                <div class="mt-8">
                  <Button variant="outline" class="w-full border-zinc-200 dark:border-zinc-800 text-zinc-600 dark:text-zinc-400 hover:text-red-600 dark:hover:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20" @click="onReport">
                    <TriangleAlert class="h-4 w-4 mr-2" />
                    Zgłoś profil
                  </Button>
                </div>
              </aside>

              <!-- Main Offers Area -->
              <section class="flex-1">
                <div class="flex flex-col gap-10">
                  <div>
                    <h2 class="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-4">Moje oferty</h2>
                    
                    <p v-if="loading" class="text-zinc-500">Ładowanie...</p>
                    <p v-else-if="error" class="text-red-500">{{ error }}</p>
                    <p v-else-if="filteredOffers.length === 0" class="text-zinc-500">Brak ofert.</p>
                    
                    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                      <RouterLink
                        v-for="o in filteredOffers"
                        :key="o.id"
                        :to="`/offer/${o.id}`"
                        class="block h-full"
                      >
                        <OfferCard :offer="o" />
                      </RouterLink>
                    </div>
                  </div>
                </div>
              </section>

            </div>
          </TabsContent>

          <!-- Reviews Tab -->
          <TabsContent value="reviews" class="pt-8">
             <div class="flex flex-col md:flex-row gap-10 md:gap-16">
              <!-- Review List -->
              <section class="flex-1 flex flex-col gap-8">
                <div v-for="r in reviews" :key="r.id" class="flex gap-4">
                  <Avatar class="h-12 w-12 border border-zinc-200 dark:border-zinc-800 shrink-0">
                    <AvatarFallback class="bg-zinc-100 dark:bg-zinc-800 text-zinc-500 font-medium">{{ r.user.charAt(0) }}</AvatarFallback>
                  </Avatar>
                  
                  <div class="flex flex-col gap-1 pt-1">
                    <div class="flex items-center gap-1 text-teal-600 dark:text-teal-400">
                      <Star v-for="i in 5" :key="i" class="h-3.5 w-3.5" :class="i <= r.stars ? 'fill-current' : 'text-zinc-300 dark:text-zinc-700'" />
                    </div>
                    <p class="text-lg md:text-xl font-bold text-zinc-800 dark:text-zinc-200 leading-snug mt-1">
                      {{ r.text }}
                    </p>
                    <span class="text-sm font-semibold text-zinc-500 dark:text-zinc-400 mt-1">{{ r.user }}</span>
                  </div>
                </div>
              </section>

              <!-- Review Stats Sidebar -->
              <aside class="md:w-72 flex flex-col gap-6 md:border-l border-zinc-200 dark:border-zinc-800 md:pl-10 shrink-0">
                <div class="flex flex-col">
                  <span class="text-sm uppercase tracking-wider font-bold text-zinc-500 dark:text-zinc-400 mb-1">Liczba opinii</span>
                  <span class="text-3xl font-black text-zinc-900 dark:text-zinc-50">{{ reviewStats.total }}</span>
                </div>

                <div class="flex flex-col">
                  <span class="text-sm uppercase tracking-wider font-bold text-zinc-500 dark:text-zinc-400 mb-1">Średnia ocena</span>
                  <span class="text-4xl font-black text-teal-600 dark:text-teal-400">{{ reviewStats.avg }}</span>
                </div>

                <div class="flex flex-col gap-2 mt-2">
                  <div v-for="s in reviewStats.stars" :key="s.stars" class="flex items-center gap-3">
                    <div class="flex items-center text-zinc-400 w-24">
                      <Star v-for="i in 5" :key="i" class="h-4 w-4" :class="i <= s.stars ? 'fill-current text-zinc-600 dark:text-zinc-400' : 'text-zinc-200 dark:text-zinc-800'" />
                    </div>
                    <span class="text-sm font-semibold text-zinc-600 dark:text-zinc-300">{{ s.count }}</span>
                  </div>
                </div>
              </aside>
            </div>
          </TabsContent>

        </Tabs>
      </div>

    </Container>
  </div>
</template>
