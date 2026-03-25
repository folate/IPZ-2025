<script setup>
import { ref, onMounted, computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import Container from "@/components/ui/Container.vue";
import { Card, CardContent, CardHeader, CardTitle, CardDescription, CardFooter } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Button } from "@/components/ui/button";
import { Heart, Loader2, User, Image as ImageIcon, ChevronLeft, ChevronRight, Star, Calendar, Briefcase } from "lucide-vue-next";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";

const route = useRoute();
const router = useRouter();
const offerDetails = ref(null);
const error = ref("");
const isFav = ref(false);
const loading = ref(false);
const sellerProfile = ref(null);

const currentPhotoIndex = ref(0);

const photos = computed(() => {
  return offerDetails.value?.photos || [];
});

const currentPhoto = computed(() => {
  if (photos.value.length === 0) return null;
  return photos.value[currentPhotoIndex.value]?.url;
});

function nextPhoto() {
  if (photos.value.length > 0) {
    currentPhotoIndex.value = (currentPhotoIndex.value + 1) % photos.value.length;
  }
}

function prevPhoto() {
  if (photos.value.length > 0) {
    currentPhotoIndex.value = (currentPhotoIndex.value - 1 + photos.value.length) % photos.value.length;
  }
}

function setPhoto(index) {
  currentPhotoIndex.value = index;
}

const sellerDisplayName = computed(() => {
  if (sellerProfile.value && sellerProfile.value.firstName && sellerProfile.value.lastName) {
    return `${sellerProfile.value.firstName} ${sellerProfile.value.lastName}`;
  }
  return sellerProfile.value?.login || offerDetails.value?.freelancer || "Wykonawca";
});

const sellerJoinDate = computed(() => {
  if (!sellerProfile.value?.joinedDate) return "";
  const date = new Date(sellerProfile.value.joinedDate);
  return date.toLocaleDateString("pl-PL", { month: "long", year: "numeric" });
});

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
    
    if (offerDetails.value.sellerId) {
      try {
        const resSeller = await fetch(`/api/Seller/${offerDetails.value.sellerId}`);
        if (resSeller.ok) {
          sellerProfile.value = await resSeller.json();
        }
      } catch (sErr) {
        console.error("Seller Profile Fetch Error:", sErr);
      }
    }
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
  router.push({
    name: 'payment',
    query: {
      gigId: tier.id,
      sellerId: offerDetails.value.freelancerId,
      price: tier.price,
      tierName: tier.tierName,
      title: offerDetails.value.title
    }
  });
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
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20">

    <Container class="animate-in fade-in slide-in-from-bottom-6 duration-700 ease-out fill-mode-both">
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
              
              <!-- Image Carousel -->
              <div class="flex flex-col gap-4">
                <div class="rounded-2xl overflow-hidden bg-zinc-100 dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 aspect-video flex items-center justify-center relative shadow-sm group">
                  <img v-if="currentPhoto" :src="currentPhoto" alt="Offer Banner" class="w-full h-full object-cover transition-opacity duration-300" />
                  <div v-else class="flex flex-col items-center justify-center text-zinc-400">
                    <ImageIcon class="h-16 w-16 mb-2 opacity-50" />
                    <span class="font-medium tracking-wide">Brak podglądu</span>
                  </div>

                  <template v-if="photos.length > 1">
                    <button @click.prevent="prevPhoto" class="absolute left-4 top-1/2 -translate-y-1/2 bg-white/80 hover:bg-white dark:bg-zinc-900/80 dark:hover:bg-zinc-900 text-zinc-800 dark:text-zinc-200 p-2 rounded-full shadow-md backdrop-blur-sm opacity-0 group-hover:opacity-100 transition-all">
                      <ChevronLeft class="h-6 w-6" />
                    </button>
                    <button @click.prevent="nextPhoto" class="absolute right-4 top-1/2 -translate-y-1/2 bg-white/80 hover:bg-white dark:bg-zinc-900/80 dark:hover:bg-zinc-900 text-zinc-800 dark:text-zinc-200 p-2 rounded-full shadow-md backdrop-blur-sm opacity-0 group-hover:opacity-100 transition-all">
                      <ChevronRight class="h-6 w-6" />
                    </button>
                  </template>
                </div>

                <!-- Thumbnails -->
                <div v-if="photos.length > 1" class="flex items-center gap-2 overflow-x-auto pb-2 scrollbar-hide">
                  <button 
                    v-for="(photo, index) in photos" 
                    :key="photo.id || index"
                    @click="setPhoto(index)"
                    class="relative shrink-0 w-20 h-14 rounded-lg overflow-hidden border-2 transition-all focus:outline-none focus:ring-2 focus:ring-teal-500"
                    :class="index === currentPhotoIndex ? 'border-teal-500 shadow-sm' : 'border-transparent opacity-60 hover:opacity-100'"
                  >
                    <img :src="photo.url" class="w-full h-full object-cover" />
                  </button>
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
              
              <!-- Seller Visual Card -->
              <Card v-if="sellerProfile" class="mt-8 border-zinc-200 dark:border-zinc-800 shadow-lg shadow-teal-900/5 overflow-hidden">
                <CardHeader class="pb-4 bg-zinc-50/50 dark:bg-zinc-900/20 border-b border-zinc-100 dark:border-zinc-800">
                  <CardTitle class="text-xl flex items-center gap-2">
                    <User class="h-5 w-5 text-teal-600" />
                    Wizytówka Wykonawcy
                  </CardTitle>
                </CardHeader>
                <CardContent class="p-6">
                  <div class="flex flex-col sm:flex-row gap-6 items-center sm:items-start text-center sm:text-left">
                    <Avatar class="h-20 w-20 border-2 border-white dark:border-zinc-800 shadow-md">
                      <AvatarFallback class="text-2xl bg-teal-100 text-teal-700 dark:bg-teal-900/30 dark:text-teal-400 font-bold">
                        {{ sellerDisplayName.charAt(0).toUpperCase() }}
                      </AvatarFallback>
                    </Avatar>
                    
                    <div class="flex-1 flex flex-col gap-3">
                      <div>
                        <h3 class="text-2xl font-bold text-zinc-900 dark:text-zinc-50 leading-tight">
                          {{ sellerDisplayName }}
                        </h3>
                        <div class="flex items-center justify-center sm:justify-start gap-1.5 mt-1">
                          <div class="flex items-center text-amber-400">
                            <Star class="h-4 w-4 fill-current" />
                            <span class="ml-1 text-zinc-900 dark:text-zinc-100 font-bold text-sm">{{ Number(sellerProfile.rating || 0).toFixed(1) }}</span>
                          </div>
                          <span class="text-xs text-zinc-500 font-medium">({{ sellerProfile.totalReviews || 0 }} opinii)</span>
                        </div>
                      </div>
                      
                      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div class="flex items-center gap-2 text-zinc-600 dark:text-zinc-400">
                          <Calendar class="h-4 w-4 text-teal-600 shrink-0" />
                          <div class="flex flex-col">
                            <span class="text-[10px] uppercase font-bold tracking-wider text-zinc-400">W systemie od</span>
                            <span class="text-xs font-semibold">{{ sellerJoinDate }}</span>
                          </div>
                        </div>
                        <div v-if="sellerProfile.skills" class="flex items-center gap-2 text-zinc-600 dark:text-zinc-400">
                          <Briefcase class="h-4 w-4 text-teal-600 shrink-0" />
                          <div class="flex flex-col min-w-0">
                            <span class="text-[10px] uppercase font-bold tracking-wider text-zinc-400">Główne atuty</span>
                            <span class="text-xs font-semibold truncate max-w-[150px]" :title="sellerProfile.skills">{{ sellerProfile.skills }}</span>
                          </div>
                        </div>
                      </div>
                      
                      <div class="pt-2 flex flex-col gap-2">
                        <p class="text-sm text-zinc-500 dark:text-zinc-400 line-clamp-2 italic">
                          "{{ sellerProfile.bio || 'Ten wykonawca jeszcze nie uzupełnił swojego opisu.' }}"
                        </p>
                        <RouterLink :to="`/seller/profile/${offerDetails.sellerId}`" class="text-teal-600 dark:text-teal-400 text-sm font-bold hover:underline flex items-center gap-1">
                          Zobacz pełny profil i inne usługi &rarr;
                        </RouterLink>
                      </div>
                    </div>
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
                        
                        <p class="text-zinc-600 dark:text-zinc-400 text-sm leading-relaxed min-h-[60px] font-medium whitespace-pre-wrap">
                          {{ tier.tierDescription }}
                        </p>
                        
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
