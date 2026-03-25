<script setup>
import { ref, computed, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useAlert } from "@/stores/alert";
import { useAuth } from "@/stores/auth";
import { useChat } from "@/stores/chat";
import { Search, Heart, MessageCircle, Star, AlertTriangle, User, Calendar, MapPin, Briefcase, Send } from "lucide-vue-next";

import Container from "../../components/ui/Container.vue";
import OfferCard from "@/components/landing/OfferCard.vue";

import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { ExternalLink, RefreshCw } from "lucide-vue-next";

const router = useRouter();
const route = useRoute();
const { showAlert } = useAlert();
const { isLoggedIn, role, state: authState } = useAuth();
const activeTab = ref("offers");
const loading = ref(false);
const error = ref("");
const searchText = ref("");
const isFavourite = ref(false);
const isStartingChat = ref(false);

const chat = useChat();

const me = ref(null);
const offers = ref([]);
const myorders = ref([]);
const isOwnProfile = ref(true);

// Edit Profile Modal Refs
const showEditProfileModal = ref(false);
const savingProfile = ref(false);
const editForm = ref({ firstName: "", lastName: "", bio: "", skills: "", hourlyRate: 0, portfolioUrl: "" });

const displayName = computed(() => {
  if (me.value && me.value.firstName && me.value.lastName) {
    return `${me.value.firstName} ${me.value.lastName}`;
  }
  return me.value?.login ?? "Sprzedawca";
});

const filteredOffers = computed(() => {
  const q = searchText.value.trim().toLowerCase();
  if (!q) return offers.value || [];
  return (offers.value || []).filter(o => {
    return String(o?.title ?? "").toLowerCase().includes(q) || String(o?.description ?? "").toLowerCase().includes(q);
  });
});

const joinDateFormatted = computed(() => {
  if (!me.value?.joinedDate) return 'Brak danych';
  const date = new Date(me.value.joinedDate);
  return date.toLocaleDateString("pl-PL", { month: "long", year: "numeric" });
});

async function load() {
  loading.value = true;
  error.value = "";
  try {
    const id = route.params.id;
    isOwnProfile.value = !id;

    const userUrl = isOwnProfile.value ? "/api/Seller/me" : `/api/Seller/${id}`;
    const resUser = await fetch(userUrl, { credentials: "include" });
    if (!resUser.ok) throw new Error("Nie udało się pobrać profilu.");
    const userData = await resUser.json();
    me.value = userData;

    await fetchReviews();

    const adsUrl = isOwnProfile.value ? "/api/SellerAd/UserAds" : `/api/SellerAd/freelancer/${userData.userId}`;
    const resOffers = await fetch(adsUrl, { credentials: "include" });
    if (!resOffers.ok) throw new Error("Błąd pobierania ofert.");
    
    const all = await resOffers.json();
    offers.value = Array.isArray(all) ? all : [];
    
    if (isOwnProfile.value) {
      const resOrders = await fetch("/api/Order/sellerorders", { credentials: "include" });
      if (resOrders.ok) {
          const ords = await resOrders.json();
          myorders.value = Array.isArray(ords) ? ords : [];
      }
    } else {
      myorders.value = [];
    }
  } catch (e) {
    error.value = e.message;
  } finally {
    loading.value = false;
  }
}

function openEditProfile() {
  editForm.value = {
    firstName: me.value?.firstName || "",
    lastName: me.value?.lastName || "",
    bio: me.value?.bio || "",
    skills: me.value?.skills || "",
    hourlyRate: me.value?.hourlyRate || 0,
    portfolioUrl: me.value?.portfolioUrl || ""
  };
  showEditProfileModal.value = true;
}

async function saveProfile() {
  savingProfile.value = true;
  try {
    const res = await fetch("/api/Seller/me", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        firstName: editForm.value.firstName,
        lastName: editForm.value.lastName,
        bio: editForm.value.bio,
        skills: editForm.value.skills,
        hourlyRate: editForm.value.hourlyRate,
        portfolioUrl: editForm.value.portfolioUrl
      }),
      credentials: "include"
    });
    if (!res.ok) throw new Error("Błąd podczas zapisywania profilu.");
    await load();
    showEditProfileModal.value = false;
  } catch (err) {
    showAlert("Błąd", err.message, "destructive");
  } finally {
    savingProfile.value = false;
  }
}

onMounted(() => {
  load();
});

function onToggleFavourite() { isFavourite.value = !isFavourite.value; }

async function onChatClick() { 
  if (!me.value?.userId) return;
  isStartingChat.value = true;
  try {
    const conversationId = await chat.getOrCreateConversation(me.value.userId);
    if (conversationId) {
      router.push(`/chat/${conversationId}`);
    } else {
      showAlert("Błąd", "Nie udało się rozpocząć konwersacji.", "destructive");
    }
  } finally {
    isStartingChat.value = false;
  }
}

const reviewStats = computed(() => ({
  total: me.value?.totalReviews || 0,
  avg: me.value?.rating || 0
}));
const reviews = ref([]);

// New Review Form Refs
const newReview = ref({ rating: 5, description: "" });
const submittingReview = ref(false);

const canLeaveReview = computed(() => {
  return isLoggedIn.value && !isOwnProfile.value && authState.user?.id !== me.value?.userId;
});

async function submitReview() {
  if (!newReview.value.description.trim()) {
    showAlert("Błąd", "Opis opinii nie może być pusty.", "destructive");
    return;
  }
  
  submittingReview.value = true;
  try {
    const res = await fetch(`/api/Seller/${me.value.id}/reviews`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        rating: newReview.value.rating,
        description: newReview.value.description
      }),
      credentials: "include"
    });
    
    if (!res.ok) {
      const txt = await res.text();
      throw new Error(txt || "Błąd podczas dodawania opinii.");
    }
    
    showAlert("Sukces", "Twoja opinia została dodana.", "default");
    newReview.value = { rating: 5, description: "" };
    await fetchReviews();
    // Also reload seller data to update the average rating
    const userUrl = `/api/Seller/${me.value.id}`;
    const resUser = await fetch(userUrl, { credentials: "include" });
    if (resUser.ok) {
      me.value = await resUser.json();
    }
  } catch (err) {
    showAlert("Błąd", err.message, "destructive");
  } finally {
    submittingReview.value = false;
  }
}

async function fetchReviews() {
  try {
    const id = route.params.id;
    if (!id && !me.value?.id) return;
    const sellerId = id || me.value.id;
    const res = await fetch(`/api/Seller/${sellerId}/reviews`, { credentials: "include" });
    if (res.ok) {
      reviews.value = await res.json();
    }
  } catch (e) {
    console.error("Error fetching reviews:", e);
  }
}

const openOrderRevision = (id) => {
  router.push(`/order/${id}/revision`);
};
</script>

<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20">

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
             <Button v-if="!isOwnProfile" class="bg-teal-600 hover:bg-teal-700 text-white shadow-sm gap-2" @click="onChatClick" :disabled="isStartingChat">
               <span v-if="isStartingChat" class="animate-spin h-4 w-4 border-2 border-white border-t-transparent rounded-full"></span>
               <MessageCircle v-else class="h-4 w-4" />
               Napisz wiadomość
             </Button>
             <Button v-else @click="openEditProfile" variant="outline" class="border-teal-200 text-teal-700 hover:bg-teal-50 dark:border-teal-900/50 dark:text-teal-400 dark:hover:bg-teal-900/20 shadow-sm gap-2">
               Edytuj profil
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
                    <span class="ml-1 text-zinc-900 dark:text-zinc-50 font-bold text-lg">{{ Number(reviewStats.avg).toFixed(1) }}</span>
                  </div>
                  <span class="text-sm">({{ reviewStats.total }} wystawionych opinii)</span>
                </div>
              </div>
              
              <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mt-2">
                <div class="flex items-center gap-3 bg-zinc-50 dark:bg-zinc-900/50 p-3 rounded-xl border border-zinc-100 dark:border-zinc-800/50 overflow-hidden">
                  <Calendar class="h-5 w-5 text-teal-600 dark:text-teal-400 shrink-0" />
                  <div class="flex flex-col text-left">
                    <span class="text-[10px] whitespace-nowrap font-bold uppercase tracking-wider text-zinc-500">Na platformie od</span>
                    <span class="text-sm font-semibold text-zinc-900 dark:text-zinc-100 truncate flex-1">{{ joinDateFormatted }}</span>
                  </div>
                </div>

                <div class="flex items-center gap-3 bg-zinc-50 dark:bg-zinc-900/50 p-3 rounded-xl border border-zinc-100 dark:border-zinc-800/50 overflow-hidden">
                  <Briefcase class="h-5 w-5 text-teal-600 dark:text-teal-400 shrink-0" />
                  <div class="flex flex-col text-left min-w-0">
                    <span class="text-[10px] font-bold uppercase tracking-wider text-zinc-500">Umiejętności</span>
                    <span class="text-sm font-semibold text-zinc-900 dark:text-zinc-100 truncate w-32" :title="me?.skills ?? 'Brak'">{{ me?.skills && me.skills.length > 0 ? me.skills : 'Brak' }}</span>
                  </div>
                </div>

                <div class="flex items-center gap-3 bg-zinc-50 dark:bg-zinc-900/50 p-3 rounded-xl border border-zinc-100 dark:border-zinc-800/50 overflow-hidden">
                  <Briefcase class="h-5 w-5 text-teal-600 dark:text-teal-400 shrink-0" />
                  <div class="flex flex-col text-left min-w-0">
                    <span class="text-[10px] whitespace-nowrap font-bold uppercase tracking-wider text-zinc-500">Zrealizowane zlecenia</span>
                    <span class="text-sm font-semibold text-zinc-900 dark:text-zinc-100 truncate">{{ me?.completedJobs ?? 0 }}</span>
                  </div>
                </div>

                <div class="flex items-center gap-3 bg-zinc-50 dark:bg-zinc-900/50 p-3 rounded-xl border border-zinc-100 dark:border-zinc-800/50 overflow-hidden">
                  <Briefcase class="h-5 w-5 text-teal-600 dark:text-teal-400 shrink-0" />
                  <div class="flex flex-col text-left min-w-0">
                    <span class="text-[10px] whitespace-nowrap font-bold uppercase tracking-wider text-zinc-500">Stawka godzinowa</span>
                    <span class="text-sm font-semibold text-zinc-900 dark:text-zinc-100 text-teal-600 truncate">{{ me?.hourlyRate ? `${me.hourlyRate} zł / h` : 'Do negocjacji' }}</span>
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
                <TabsTrigger v-if="isOwnProfile" value="orders" class="rounded-none border-b-2 border-transparent px-2 py-4 text-base font-bold text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100 data-[state=active]:border-teal-600 data-[state=active]:text-teal-700 dark:data-[state=active]:text-teal-400 data-[state=active]:shadow-none data-[state=active]:bg-transparent transition-colors">
                  Zamówienia w Trakcie ({{ myorders.filter(o => o.status !== 'Completed').length }})
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

              <!-- Orders Tab -->
              <TabsContent v-if="isOwnProfile" value="orders" class="m-0 focus-visible:outline-none flex flex-col gap-6">
                <div v-if="myorders.filter(o => o.status !== 'Completed').length === 0" class="flex flex-col items-center justify-center p-10 border-2 border-dashed border-zinc-200 dark:border-zinc-800 rounded-xl bg-zinc-50 dark:bg-zinc-900/30 text-zinc-500">
                  <Briefcase class="h-10 w-10 mb-3 opacity-50" />
                  <span class="font-medium text-lg text-zinc-900 dark:text-zinc-200">Brak zamówień</span>
                  <span class="text-sm max-w-sm text-center">Nie masz obecnie przypisanych żadnych aktywnych zamówień.</span>
                </div>
                
                <div v-else class="flex flex-col gap-4">
                  <div 
                    v-for="order in myorders.filter(o => o.status !== 'Completed')" :key="order.id"
                    class="group relative bg-white dark:bg-zinc-900 rounded-xl p-5 border border-zinc-200 dark:border-zinc-800 shadow-sm hover:shadow-md hover:border-teal-200 dark:hover:border-teal-900/50 transition-all cursor-pointer flex flex-col md:flex-row md:items-center justify-between gap-4"
                    @click="openOrderRevision(order.id)"
                  >
                    <div class="flex items-center gap-4">
                      <div class="h-12 w-12 shrink-0 rounded-lg bg-indigo-50 dark:bg-indigo-900/30 flex items-center justify-center text-indigo-600 dark:text-indigo-400">
                        <RefreshCw class="h-6 w-6 font-bold" />
                      </div>
                      <div class="flex flex-col">
                        <span class="text-sm font-bold text-zinc-400">Zamówienie #{{ order.id }}</span>
                        <h4 class="font-bold text-zinc-900 dark:text-zinc-100 text-lg">
                           {{ order.gig?.title || (order.status === 'Paid' ? 'Nowe zlecenie do realizacji' : 'Szczegóły zlecenia') }}
                        </h4>
                        <span class="text-sm text-zinc-500 dark:text-zinc-400 font-medium whitespace-nowrap">Cena: {{ order.price }} zł</span>
                      </div>
                    </div>
                    
                    <div class="flex flex-wrap items-center gap-4 md:border-l border-zinc-100 dark:border-zinc-800 md:pl-6 min-w-40">
                      <div class="flex flex-col">
                        <span class="text-sm text-zinc-500">Status</span>
                        <span class="font-bold text-indigo-600 dark:text-indigo-400 flex items-center gap-1.5">
                          {{ order.status }}
                        </span>
                      </div>
                      <Button variant="ghost" size="icon" class="ml-auto text-zinc-400 group-hover:text-indigo-600 transition-colors">
                        <ExternalLink class="h-5 w-5" />
                      </Button>
                    </div>
                  </div>
                </div>
              </TabsContent>

              <!-- Reviews Tab -->
              <TabsContent value="reviews" class="m-0 focus-visible:outline-none flex flex-col gap-8">
                
                <!-- Add Review Form -->
                <div v-if="canLeaveReview" class="bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-2xl p-6 shadow-sm">
                  <h3 class="text-xl font-bold text-zinc-900 dark:text-zinc-50 mb-4">Wystaw opinię</h3>
                  <div class="flex flex-col gap-4">
                    <div class="flex items-center gap-2">
                      <span class="text-sm font-medium text-zinc-500 mr-2">Twoja ocena:</span>
                      <div class="flex items-center gap-1">
                        <button 
                          v-for="i in 5" :key="i" 
                          @click="newReview.rating = i"
                          class="focus:outline-none transition-transform active:scale-90"
                        >
                          <Star 
                            class="h-6 w-6" 
                            :class="i <= newReview.rating ? 'fill-teal-500 text-teal-500' : 'text-zinc-300 dark:text-zinc-700'" 
                          />
                        </button>
                      </div>
                    </div>
                    <textarea 
                      v-model="newReview.description" 
                      placeholder="Napisz kilka słów o współpracy z tym wykonawcą..." 
                      class="w-full rounded-xl border-zinc-200 dark:border-zinc-800 bg-zinc-50 dark:bg-zinc-950 px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-teal-500 min-h-[120px] resize-none"
                    ></textarea>
                    <div class="flex justify-end">
                      <Button 
                        @click="submitReview" 
                        :disabled="submittingReview"
                        class="bg-teal-600 hover:bg-teal-700 text-white px-6 py-2 rounded-xl flex items-center gap-2 shadow-md shadow-teal-900/10"
                      >
                        <Send v-if="!submittingReview" class="h-4 w-4" />
                        <span v-if="submittingReview" class="animate-spin h-4 w-4 border-2 border-white border-t-transparent rounded-full"></span>
                        {{ submittingReview ? 'Wysyłanie...' : 'Opublikuj opinię' }}
                      </Button>
                    </div>
                  </div>
                </div>

                <div v-if="reviews.length === 0" class="flex flex-col items-center justify-center p-10 border-2 border-dashed border-zinc-200 dark:border-zinc-800 rounded-xl bg-zinc-50 dark:bg-zinc-900/30 text-zinc-500">
                  <MessageCircle class="h-10 w-10 mb-3 opacity-50" />
                  <span class="font-medium text-lg text-zinc-900 dark:text-zinc-200">Brak opinii</span>
                  <span class="text-sm">Ten wykonawca nie otrzymał jeszcze żadnych recenzji.</span>
                </div>
                <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <Card v-for="r in reviews" :key="r.id" class="shadow-sm border-zinc-200 dark:border-zinc-800 bg-zinc-50 dark:bg-zinc-900/30">
                    <CardContent class="p-6 flex flex-col gap-3">
                      <div class="flex items-center justify-between">
                         <span class="font-bold text-lg text-zinc-900 dark:text-zinc-100 flex items-center gap-2">
                           <Avatar class="h-8 w-8"><AvatarFallback class="bg-zinc-200 dark:bg-zinc-800 text-xs">{{ (r.buyerName || 'U').charAt(0) }}</AvatarFallback></Avatar>
                           {{ r.buyerName }}
                         </span>
                         <div class="flex items-center text-teal-600">
                           <Star v-for="i in 5" :key="i" class="h-4 w-4" :class="i <= r.rating ? 'fill-current' : 'text-zinc-300 dark:text-zinc-700'" />
                         </div>
                      </div>
                      <p class="text-zinc-700 dark:text-zinc-300">{{ r.description }}</p>
                      <div class="text-[10px] text-zinc-400 mt-1">
                        {{ new Date(r.createdAt).toLocaleDateString('pl-PL') }}
                      </div>
                    </CardContent>
                  </Card>
                </div>
              </TabsContent>

              <!-- About Tab -->
              <TabsContent value="about" class="m-0 focus-visible:outline-none flex flex-col gap-6">
                 <div class="prose dark:prose-invert max-w-4xl text-zinc-700 dark:text-zinc-300 text-lg leading-relaxed">
                   <p>{{ me?.bio ?? 'Opis niedostępny. Ten wykonawca jeszcze nie uzupełnił swojego bio.' }}</p>
                 </div>
                 
                 <div class="mt-6 pt-6 border-t border-zinc-100 dark:border-zinc-800">
                    <Button variant="ghost" class="text-zinc-500 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20" @click="showAlert('Zgłoszono', 'Profil został zgłoszony do moderacji.')">
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

  <!-- Edit Profile Modal -->
  <div v-if="showEditProfileModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm">
    <div class="bg-white dark:bg-zinc-950 rounded-2xl shadow-xl w-full max-w-lg overflow-hidden flex flex-col border border-zinc-200 dark:border-zinc-800">
      <div class="p-6 border-b border-zinc-100 dark:border-zinc-800 flex justify-between items-center">
        <h3 class="text-xl font-bold text-zinc-900 dark:text-zinc-50">Edytuj profil</h3>
        <button @click="showEditProfileModal = false" class="text-zinc-400 hover:text-zinc-600 dark:hover:text-zinc-200">&times;</button>
      </div>
      <div class="p-6 flex flex-col gap-4 overflow-y-auto max-h-[60vh]">
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-semibold mb-1 text-zinc-700 dark:text-zinc-300">Imię</label>
            <input v-model="editForm.firstName" type="text" class="w-full rounded-xl border-zinc-200 dark:border-zinc-800 bg-transparent px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-teal-500" />
          </div>
          <div>
            <label class="block text-sm font-semibold mb-1 text-zinc-700 dark:text-zinc-300">Nazwisko</label>
            <input v-model="editForm.lastName" type="text" class="w-full rounded-xl border-zinc-200 dark:border-zinc-800 bg-transparent px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-teal-500" />
          </div>
        </div>
        <div>
          <label class="block text-sm font-semibold mb-1 text-zinc-700 dark:text-zinc-300">Skrócony opis (Bio)</label>
          <textarea v-model="editForm.bio" class="w-full rounded-xl border-zinc-200 dark:border-zinc-800 bg-transparent px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-teal-500 min-h-[100px]"></textarea>
        </div>
        <div>
          <label class="block text-sm font-semibold mb-1 text-zinc-700 dark:text-zinc-300">Umiejętności (rozdzielone przecinkami)</label>
          <input v-model="editForm.skills" type="text" class="w-full rounded-xl border-zinc-200 dark:border-zinc-800 bg-transparent px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-teal-500" />
        </div>
        <div>
          <label class="block text-sm font-semibold mb-1 text-zinc-700 dark:text-zinc-300">Stawka godzinowa (zł)</label>
          <input v-model.number="editForm.hourlyRate" type="number" class="w-full rounded-xl border-zinc-200 dark:border-zinc-800 bg-transparent px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-teal-500" />
        </div>
        <div>
          <label class="block text-sm font-semibold mb-1 text-zinc-700 dark:text-zinc-300">URL Portfolio</label>
          <input v-model="editForm.portfolioUrl" type="url" class="w-full rounded-xl border-zinc-200 dark:border-zinc-800 bg-transparent px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-teal-500" />
        </div>
      </div>
      <div class="p-6 border-t border-zinc-100 dark:border-zinc-800 flex justify-end gap-3">
        <Button variant="outline" @click="showEditProfileModal = false">Anuluj</Button>
        <Button class="bg-teal-600 hover:bg-teal-700 text-white" @click="saveProfile" :disabled="savingProfile">
          {{ savingProfile ? 'Zapisywanie...' : 'Zapisz zmiany' }}
        </Button>
      </div>
    </div>
  </div>
</template>
