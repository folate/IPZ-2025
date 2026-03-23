<script setup>
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { Search, Heart, MessageCircle, Star, AlertTriangle, User, Calendar, MapPin, Briefcase } from "lucide-vue-next";

import LandingHeader from "../../components/landing/LandingHeader.vue";
import Container from "../../components/ui/Container.vue";
import OfferCard from "@/components/landing/OfferCard.vue";

import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { ExternalLink, RefreshCw } from "lucide-vue-next";

const router = useRouter();
const activeTab = ref("offers");
const loading = ref(false);
const error = ref("");
const searchText = ref("");
const isFavourite = ref(false);

const me = ref(null);
const offers = ref([]);
const displayName = computed(() => me.value?.login ?? "Sprzedawca");

function offerOwnerLogin(o) {
  return o?.freelancer ?? o?.freelancerLogin ?? o?.seller ?? o?.sellerLogin ?? o?.login ?? null;
}

const filteredOffers = computed(() => {
  const q = searchText.value.trim().toLowerCase();
  if (!q) return offers.value || [];
  return (offers.value || []).filter(o => {
    return String(o?.title ?? "").toLowerCase().includes(q) || String(o?.description ?? "").toLowerCase().includes(q);
  });
});

async function load() {
  loading.value = true;
  error.value = "";
  try {
    const resUser = await fetch("/api/User/getUser", { credentials: "include" });
    if (!resUser.ok) throw new Error("Nie jesteś zalogowany.");
    const userData = await resUser.json();
    me.value = userData;

    const resOffers = await fetch("/api/sellerad/all/200", { credentials: "include" });
    if (!resOffers.ok) throw new Error("Błąd pobierania ofert.");
    
    const all = await resOffers.json();
    offers.value = (Array.isArray(all) ? all : []).filter(o => offerOwnerLogin(o) === userData.login);
  } catch (e) {
    error.value = e.message;
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  load();
});

function onToggleFavourite() { isFavourite.value = !isFavourite.value; }
function onChatClick() { console.log("chat click"); }

// Mocks
const reviewStats = { total: 42, avg: 4.8 };
const reviews = [
  { id: 1, user: "Jan", stars: 5, text: "Świetna współpraca, polecam z całego serca!" },
  { id: 2, user: "Anna", stars: 4, text: "Dobry kontakt i wysoka jakość." },
];

const openOrderRevision = (id) => {
  router.push(`/order/${id}/revision`);
};
</script>

<template>
  <div class="min-h-svh bg-zinc-50 dark:bg-zinc-950 pb-20">
    <LandingHeader />

    <Container>
      <div class="mt-8 w-full flex flex-col gap-6">
        
        <!-- Action Header -->
        <div class="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
          <div>
            <h1 class="text-3xl font-extrabold text-zinc-900 dark:text-zinc-50">Profil Wykonawcy</h1>
            <p class="text-zinc-500 dark:text-zinc-400 mt-1">Przeglądaj oferty, kwalifikacje i opinie.</p>
          </div>
          <div class="flex items-center gap-3">
             <Button variant="outline" @click="onToggleFavourite" class="border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 shadow-sm gap-2">
               <Heart class="h-4 w-4" :class="isFavourite ? 'fill-rose-500 text-rose-500' : 'text-zinc-500'" />
               <span>{{ isFavourite ? 'Zapisano profil' : 'Zapisz profil' }}</span>
             </Button>
             <Button class="bg-teal-600 hover:bg-teal-700 text-white shadow-sm gap-2" @click="onChatClick">
               <MessageCircle class="h-4 w-4" />
               Napisz wiadomość
             </Button>
          </div>
        </div>

        <!-- Main Info Card -->
        <Card class="w-full shadow-lg shadow-teal-900/5 border-zinc-200 dark:border-zinc-800 overflow-hidden relative">
          <CardContent class="p-8 md:p-10 flex flex-col md:flex-row gap-8 items-center md:items-start text-center md:text-left">
            <Avatar class="h-28 w-28 md:h-32 md:w-32 border-4 border-zinc-100 dark:border-zinc-900 shadow-xl bg-teal-50 dark:bg-teal-900/20">
              <AvatarFallback class="text-4xl text-teal-700 dark:text-teal-400 font-bold bg-zinc-100 dark:bg-zinc-800">
                {{ displayName.charAt(0).toUpperCase() }}
              </AvatarFallback>
            </Avatar>
            
            <div class="flex-1 flex flex-col gap-4">
              <div class="flex flex-col gap-1">
                <h2 class="text-3xl sm:text-4xl font-black text-zinc-900 dark:text-zinc-50">
                  {{ displayName }}
                </h2>
                <div class="flex items-center justify-center md:justify-start gap-2 text-zinc-600 dark:text-zinc-400 font-medium">
                  <div class="flex items-center text-teal-600 dark:text-teal-400">
                    <Star class="h-5 w-5 fill-current" />
                    <span class="ml-1 text-zinc-900 dark:text-zinc-50 font-bold text-lg">{{ reviewStats.avg }}</span>
                  </div>
                  <span class="text-sm">({{ reviewStats.total }} wystawionych opinii)</span>
                </div>
              </div>
              
              <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mt-2">
                <div class="flex items-center gap-3 bg-zinc-50 dark:bg-zinc-900/50 p-3 rounded-xl border border-zinc-100 dark:border-zinc-800/50">
                  <MapPin class="h-5 w-5 text-teal-600 dark:text-teal-400 shrink-0" />
                  <div class="flex flex-col text-left">
                    <span class="text-[10px] font-bold uppercase tracking-wider text-zinc-500">Lokalizacja</span>
                    <span class="text-sm font-semibold text-zinc-900 dark:text-zinc-100">Polska (Remote)</span>
                  </div>
                </div>

                <div class="flex items-center gap-3 bg-zinc-50 dark:bg-zinc-900/50 p-3 rounded-xl border border-zinc-100 dark:border-zinc-800/50">
                  <Calendar class="h-5 w-5 text-teal-600 dark:text-teal-400 shrink-0" />
                  <div class="flex flex-col text-left">
                    <span class="text-[10px] font-bold uppercase tracking-wider text-zinc-500">Na platformie od</span>
                    <span class="text-sm font-semibold text-zinc-900 dark:text-zinc-100">Styczeń 2024</span>
                  </div>
                </div>

                <div class="flex items-center gap-3 bg-zinc-50 dark:bg-zinc-900/50 p-3 rounded-xl border border-zinc-100 dark:border-zinc-800/50">
                  <Briefcase class="h-5 w-5 text-teal-600 dark:text-teal-400 shrink-0" />
                  <div class="flex flex-col text-left">
                    <span class="text-[10px] font-bold uppercase tracking-wider text-zinc-500">Specjalizacja</span>
                    <span class="text-sm font-semibold text-zinc-900 dark:text-zinc-100">Web Development</span>
                  </div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <!-- Tabs Content -->
        <Card class="w-full shadow-lg shadow-teal-900/5 border-zinc-200 dark:border-zinc-800 overflow-hidden">
          <Tabs defaultValue="offers" class="w-full h-full flex flex-col">
            <div class="border-b border-zinc-200 dark:border-zinc-800 px-6 pt-4 bg-zinc-50/50 dark:bg-zinc-900/20">
              <TabsList class="flex h-auto p-0 bg-transparent rounded-none gap-8">
                <TabsTrigger value="offers" class="rounded-none border-b-2 border-transparent px-2 py-4 text-base font-bold text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100 data-[state=active]:border-teal-600 data-[state=active]:text-teal-700 dark:data-[state=active]:text-teal-400 data-[state=active]:shadow-none data-[state=active]:bg-transparent transition-colors">
                  Oferty Wykonawcy
                </TabsTrigger>
                <TabsTrigger value="orders" class="rounded-none border-b-2 border-transparent px-2 py-4 text-base font-bold text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100 data-[state=active]:border-teal-600 data-[state=active]:text-teal-700 dark:data-[state=active]:text-teal-400 data-[state=active]:shadow-none data-[state=active]:bg-transparent transition-colors">
                  Zamówienia w Trakcie (1)
                </TabsTrigger>
                <TabsTrigger value="reviews" class="rounded-none border-b-2 border-transparent px-2 py-4 text-base font-bold text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100 data-[state=active]:border-teal-600 data-[state=active]:text-teal-700 dark:data-[state=active]:text-teal-400 data-[state=active]:shadow-none data-[state=active]:bg-transparent transition-colors">
                  Opinie ({{ reviewStats.total }})
                </TabsTrigger>
                <TabsTrigger value="about" class="rounded-none border-b-2 border-transparent px-2 py-4 text-base font-bold text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100 data-[state=active]:border-teal-600 data-[state=active]:text-teal-700 dark:data-[state=active]:text-teal-400 data-[state=active]:shadow-none data-[state=active]:bg-transparent transition-colors">
                  O profilu
                </TabsTrigger>
              </TabsList>
            </div>

            <div class="p-6 md:p-8 flex-1">
              <!-- Offers Tab -->
              <TabsContent value="offers" class="m-0 focus-visible:outline-none flex flex-col gap-6">
                <div class="relative w-full max-w-sm">
                  <Search class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-zinc-400" />
                  <Input v-model="searchText" placeholder="Wyszukaj ofertę uwzględniając słowa kluczowe..." class="pl-9 h-11 bg-zinc-50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus-visible:ring-teal-500 rounded-xl" />
                </div>
                
                <div v-if="loading" class="flex justify-center p-10"><div class="animate-spin h-8 w-8 border-4 border-teal-600 border-t-transparent rounded-full"></div></div>
                <div v-else-if="error" class="p-4 bg-red-50 text-red-600 rounded-xl font-medium">{{error}}</div>
                <div v-else-if="filteredOffers.length === 0" class="flex flex-col items-center justify-center p-10 border-2 border-dashed border-zinc-200 dark:border-zinc-800 rounded-xl bg-zinc-50 dark:bg-zinc-900/30 text-zinc-500">
                  <Search class="h-10 w-10 mb-3 opacity-50" />
                  <span class="font-medium text-lg text-zinc-900 dark:text-zinc-200">Brak ofert</span>
                  <span class="text-sm">Ten wykonawca obecnie nie prowadzi żadnych projektów.</span>
                </div>
                <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                  <RouterLink v-for="o in filteredOffers" :key="o.id" :to="`/offer/${o.id}`" class="block">
                    <OfferCard :offer="o" />
                  </RouterLink>
                </div>
              </TabsContent>

              <!-- Orders Tab (MOCK) -->
              <TabsContent value="orders" class="m-0 focus-visible:outline-none flex flex-col gap-6">
                <!-- Placeholder Order -->
                <div 
                  class="group relative bg-white dark:bg-zinc-900 rounded-xl p-5 border border-zinc-200 dark:border-zinc-800 shadow-sm hover:shadow-md hover:border-teal-200 dark:hover:border-teal-900/50 transition-all cursor-pointer flex flex-col md:flex-row md:items-center justify-between gap-4"
                  @click="openOrderRevision('ORD-001')"
                >
                  <div class="flex items-center gap-4">
                    <div class="h-12 w-12 shrink-0 rounded-lg bg-indigo-50 dark:bg-indigo-900/30 flex items-center justify-center text-indigo-600 dark:text-indigo-400">
                      <RefreshCw class="h-6 w-6 font-bold" />
                    </div>
                    <div class="flex flex-col">
                      <span class="text-sm font-bold text-zinc-400">Kupujący: Dawid Głowacki</span>
                      <h4 class="font-bold text-zinc-900 dark:text-zinc-100 text-lg">Nowoczesna strona internetowa WordPress</h4>
                      <span class="text-sm text-zinc-500 dark:text-zinc-400 font-medium">Koszt: 1500 zł | Czas: 7 dni</span>
                    </div>
                  </div>
                  
                  <div class="flex items-center gap-4 md:border-l border-zinc-100 dark:border-zinc-800 md:pl-6 min-w-40">
                    <div class="flex flex-col">
                      <span class="text-sm text-zinc-500">Twój Status</span>
                      <span class="font-bold text-indigo-600 dark:text-indigo-400 flex items-center gap-1.5">
                        Wymaga Dostarczenia
                      </span>
                    </div>
                    <Button variant="ghost" size="icon" class="ml-auto text-zinc-400 group-hover:text-indigo-600 transition-colors">
                      <ExternalLink class="h-5 w-5" />
                    </Button>
                  </div>
                </div>
              </TabsContent>

              <!-- Reviews Tab -->
              <TabsContent value="reviews" class="m-0 focus-visible:outline-none flex flex-col gap-6">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <Card v-for="r in reviews" :key="r.id" class="shadow-sm border-zinc-200 dark:border-zinc-800 bg-zinc-50 dark:bg-zinc-900/30">
                    <CardContent class="p-6 flex flex-col gap-3">
                      <div class="flex items-center justify-between">
                         <span class="font-bold text-lg text-zinc-900 dark:text-zinc-100 flex items-center gap-2">
                           <Avatar class="h-8 w-8"><AvatarFallback class="bg-zinc-200 dark:bg-zinc-800 text-xs">{{ r.user.charAt(0) }}</AvatarFallback></Avatar>
                           {{ r.user }}
                         </span>
                         <div class="flex items-center text-teal-600">
                           <Star v-for="i in 5" :key="i" class="h-4 w-4" :class="i <= r.stars ? 'fill-current' : 'text-zinc-300 dark:text-zinc-700'" />
                         </div>
                      </div>
                      <p class="text-zinc-700 dark:text-zinc-300">{{ r.text }}</p>
                    </CardContent>
                  </Card>
                </div>
              </TabsContent>

              <!-- About Tab -->
              <TabsContent value="about" class="m-0 focus-visible:outline-none flex flex-col gap-6">
                 <div class="prose dark:prose-invert max-w-4xl text-zinc-700 dark:text-zinc-300 text-lg leading-relaxed">
                   <p>Cześć! Jestem doświadczonym web deweloperem, otwartym na nowe wyzwania z pasją do programowania.</p>
                   <p>Dzięki wieloletniemu doświadczeniu buduję bardzo dobrze prosperujące sklepy i rozbudowane nowoczesne portale korzystając z nowoczesnych technologii (Vue, React, Node.js).</p>
                 </div>
                 
                 <div class="mt-6 pt-6 border-t border-zinc-100 dark:border-zinc-800">
                    <Button variant="ghost" class="text-zinc-500 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20" @click="$alert('Reported')">
                      <AlertTriangle class="h-4 w-4 mr-2" /> Raportuj ten profil
                    </Button>
                 </div>
              </TabsContent>
            </div>
          </Tabs>
        </Card>

      </div>
    </Container>
  </div>
</template>
