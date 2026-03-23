<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import LandingHeader from "@/components/landing/LandingHeader.vue";
import Container from "@/components/ui/Container.vue";
import { Card, CardContent, CardHeader, CardTitle, CardDescription, CardFooter } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Button } from "@/components/ui/button";
import { Heart, Loader2, User, Image as ImageIcon, Check } from "lucide-vue-next";

// Using a placeholder or actual image if available
import offerImage from "../../../public/Placeholders/offerImage.png";

const route = useRoute();
const offerDetails = ref(null);
const error = ref("");
const isFav = ref(false);
const loading = ref(false);

onMounted(() => {
  fetchDetails();
  checkFavs();
});

async function fetchDetails() {
  loading.value = true;
  const id = route.params.id;
  try {
    const res = await fetch(`/api/SellerAd/${id}`);
    if (!res.ok) {
      throw new Error(`Błąd pobierania szczegółów (${res.status})`);
    }
    offerDetails.value = await res.json();
  } catch (err) {
    error.value = err.message;
  } finally {
    loading.value = false;
  }
}

function checkFavs() {
  const item = localStorage.getItem("FavoritesIds");
  const favorites = item ? JSON.parse(item) : [];
  isFav.value = favorites.includes(route.params.id);
}

async function buyTier(tier) {
  console.log(tier.id, " tier bought:", tier.tierName, " ", tier.price, "zł");
  try {
    const res = await fetch(`/api/Order/create`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        quantity: 1,
        additionalInstructions: "None",
        gigId: tier.id
      }),
    });
    if (!res.ok) {
      throw new Error(`Błąd zakupu (${res.status})`);
    }
    const data = await res.json(); 
    console.log("bought successfully", data);
  } catch (err) {
    error.value = err.message;
  }
}

function addFavorites() {
  const id = route.params.id;
  const item = localStorage.getItem("FavoritesIds");
  let favorites = item ? JSON.parse(item) : [];

  const index = favorites.indexOf(id);
  if (index === -1) {
    favorites.push(id);
    isFav.value = true;
  } else {
    favorites.splice(index, 1);
    isFav.value = false;
  }
  localStorage.setItem("FavoritesIds", JSON.stringify(favorites));
}
</script>

<template>
  <div class="min-h-svh bg-zinc-50 dark:bg-zinc-950 pb-20">
    <LandingHeader />

    <Container>
      <div class="mt-8 w-full flex flex-col gap-8">
        
        <div v-if="loading" class="flex justify-center py-20">
          <Loader2 class="h-10 w-10 text-teal-600 animate-spin" />
        </div>

        <div v-else-if="error" class="bg-red-50 dark:bg-red-900/10 text-red-600 dark:text-red-400 p-6 rounded-2xl font-medium border border-red-200 dark:border-red-900/30">
          {{ error }}
        </div>

        <template v-else-if="offerDetails">
          
          <!-- Main Offer Header -->
          <div class="flex flex-col md:flex-row gap-6 items-start justify-between">
            <div class="flex flex-col gap-2 flex-1">
              <h1 class="text-3xl md:text-4xl font-extrabold text-zinc-900 dark:text-zinc-50 leading-tight">
                {{ offerDetails.title }}
              </h1>
              <div class="flex items-center gap-3 text-zinc-500 dark:text-zinc-400 mt-2">
                <div class="flex items-center gap-1.5 bg-zinc-100 dark:bg-zinc-900 px-3 py-1.5 rounded-full text-sm font-medium border border-zinc-200 dark:border-zinc-800">
                  <User class="h-4 w-4" /> {{ offerDetails.freelancer || 'Nieznany wykonawca' }}
                </div>
              </div>
            </div>

            <Button 
               variant="outline" 
               size="lg"
               @click="addFavorites" 
               class="shrink-0 h-12 px-6 rounded-xl border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 hover:bg-zinc-50 hover:border-teal-500 transition-all group shadow-sm flex items-center gap-2"
            >
              <Heart class="h-5 w-5 transition-colors" :class="isFav ? 'fill-rose-500 text-rose-500' : 'text-zinc-400 group-hover:text-rose-400'" />
              <span class="font-semibold text-zinc-700 dark:text-zinc-300" :class="isFav ? 'text-rose-600 dark:text-rose-500' : ''">
                {{ isFav ? 'Zapisano' : 'Do Ulubionych' }}
              </span>
            </Button>
          </div>

          <!-- Main Content Grid -->
          <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
            
            <!-- Left Column: Image & Details -->
            <div class="lg:col-span-2 flex flex-col gap-8">
              
              <!-- Image Banner -->
              <div class="rounded-2xl overflow-hidden bg-zinc-100 dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 aspect-video flex items-center justify-center relative shadow-sm">
                <img v-if="offerImage" :src="offerImage" alt="Offer Banner" class="w-full h-full object-cover" />
                <div v-else class="flex flex-col items-center justify-center text-zinc-400">
                  <ImageIcon class="h-16 w-16 mb-2 opacity-50" />
                  <span class="font-medium tracking-wide">Brak podglądu</span>
                </div>
              </div>

              <!-- Description Block -->
              <Card class="border-zinc-200 dark:border-zinc-800 shadow-sm">
                <CardHeader>
                  <CardTitle class="text-xl">O usłudze</CardTitle>
                </CardHeader>
                <CardContent>
                  <div class="prose dark:prose-invert max-w-none text-zinc-600 dark:text-zinc-300">
                    <p class="whitespace-pre-line leading-relaxed text-lg">
                      {{ offerDetails.description }}
                    </p>
                  </div>
                </CardContent>
              </Card>

            </div>

            <!-- Right Column: Tiers (Fiverr Style) -->
            <div class="lg:sticky lg:top-24 h-fit flex flex-col gap-6">
              <div v-if="offerDetails.gigs && offerDetails.gigs.length" class="w-full">
                <Tabs :default-value="offerDetails.gigs[0]?.tierName" class="w-full">
                  <Card class="border-zinc-200 dark:border-zinc-800 shadow-xl shadow-teal-900/5 overflow-hidden rounded-2xl">
                    <TabsList class="w-full flex h-auto p-0 bg-transparent border-b border-zinc-200 dark:border-zinc-800 rounded-none">
                      <TabsTrigger 
                        v-for="tier in offerDetails.gigs" 
                        :key="'trigger-' + tier.tierName" 
                        :value="tier.tierName"
                        class="flex-1 rounded-none border-b-2 border-transparent py-4 text-sm font-bold text-zinc-500 hover:text-zinc-900 dark:text-zinc-400 dark:hover:text-zinc-100 data-[state=active]:border-teal-600 data-[state=active]:text-teal-700 dark:data-[state=active]:text-teal-400 data-[state=active]:bg-zinc-50/50 dark:data-[state=active]:bg-zinc-900/50 transition-all uppercase tracking-wider"
                      >
                        {{ tier.tierName }}
                      </TabsTrigger>
                    </TabsList>
                    
                    <TabsContent 
                      v-for="tier in offerDetails.gigs" 
                      :key="'content-' + tier.tierName" 
                      :value="tier.tierName" 
                      class="p-0 m-0 outline-none"
                    >
                      <div class="p-6 md:p-8 flex flex-col gap-6">
                        <div class="flex justify-between items-start gap-4">
                          <h3 class="font-bold text-xl text-zinc-900 dark:text-zinc-50">{{ tier.tierName }}</h3>
                          <span class="text-3xl font-black text-teal-600 dark:text-teal-400 shrink-0">{{ tier.price }} zł</span>
                        </div>
                        
                        <p class="text-zinc-600 dark:text-zinc-400 text-sm leading-relaxed min-h-[60px] font-medium">
                          {{ tier.tierDescription }}
                        </p>
                        
                        <!-- Placeholder specific features typical of Fiverr gigs -->
                        <ul class="flex flex-col gap-3 text-sm text-zinc-600 dark:text-zinc-400 font-medium">
                           <li class="flex items-center gap-3">
                             <Check class="h-4 w-4 text-teal-600 dark:text-teal-400 shrink-0" />
                             Gwarancja jakości
                           </li>
                           <li class="flex items-center gap-3">
                             <Check class="h-4 w-4 text-teal-600 dark:text-teal-400 shrink-0" />
                             Profesjonalna realizacja
                           </li>
                        </ul>

                        <div class="pt-4">
                          <Button @click="buyTier(tier)" class="w-full h-12 bg-zinc-900 dark:bg-zinc-50 text-white dark:text-zinc-900 hover:bg-teal-600 hover:text-white dark:hover:bg-teal-500 font-bold tracking-wide transition-colors shadow-md text-base rounded-xl">
                            Wybierz ten pakiet
                          </Button>
                        </div>
                      </div>
                    </TabsContent>
                  </Card>
                </Tabs>
              </div>
              
              <div v-else class="text-zinc-500 dark:text-zinc-400 italic bg-zinc-100 dark:bg-zinc-900 p-6 rounded-xl text-center border border-zinc-200 dark:border-zinc-800">
                Wykonawca nie zdefiniował jeszcze cennika pakietów dla tej oferty.
              </div>
            </div>

          </div>
        </template>
        
      </div>
    </Container>
  </div>
</template>
