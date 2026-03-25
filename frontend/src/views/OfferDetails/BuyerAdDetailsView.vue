<script setup>
import { ref, onMounted, computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import Container from "@/components/ui/Container.vue";
import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Loader2, Calendar as CalendarIcon, DollarSign, Tag, User, Send, CheckCircle2, MessageSquare, MessageCircle, Clock } from "lucide-vue-next";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { cn } from "@/lib/utils";
import { DateFormatter, getLocalTimeZone } from '@internationalized/date';
import { useAuth } from "@/stores/auth";
import { ROLES } from "@/auth/roles";
import { useChat } from "@/stores/chat";
import { useAlert } from "@/stores/alert";

const df = new DateFormatter('pl-PL', { dateStyle: 'long' });

const route = useRoute();
const router = useRouter();
const { role, state: authState } = useAuth();
const chat = useChat();
const { showAlert } = useAlert();

const loading = ref(false);
const submitting = ref(false);
const error = ref("");
const ad = ref(null);
const myOffer = ref(null);
const isStartingChat = ref(false);

// Offer form state
const offerForm = ref({
  price: 0,
  description: ""
});
const deadlineDate = ref();

const isOwner = computed(() => ad.value && authState.user && ad.value.buyerName === authState.user.login);
const isSeller = computed(() => role.value === ROLES.SELLER);

async function load() {
  loading.value = true;
  error.value = "";
  ad.value = null;

  try {
    const res = await fetch(`/api/BuyerAd/${route.params.id}`, {
      credentials: "include",
    });

    if (res.status === 204) {
      error.value = "Nie znaleziono zlecenia.";
      return;
    }

    if (!res.ok) {
      const text = await res.text().catch(() => "");
      error.value = `Błąd pobierania (${res.status}). ${text}`;
      return;
    }

    ad.value = await res.json();
    
    // If seller, check if already submitted an offer
    if (isSeller.value) {
      await loadMyOffer();
    }
  } catch {
    error.value = "Błąd sieci przy pobieraniu zlecenia.";
  } finally {
    loading.value = false;
  }
}

async function loadMyOffer() {
  try {
    const res = await fetch(`/api/BuyerAd/${route.params.id}/offers`, {
      credentials: "include",
    });
    if (res.ok) {
      const offers = await res.json();
      if (offers && offers.length > 0) {
        myOffer.value = offers[0];
      }
    }
  } catch (e) {
    console.error("Failed to load my offer", e);
  }
}

async function submitOffer() {
  if (!offerForm.value.price || !deadlineDate.value || !offerForm.value.description) {
    showAlert("Błąd formularza", "Proszę wypełnić wszystkie pola oferty, w tym datę.", "destructive");
    return;
  }

  submitting.value = true;
  try {
    const res = await fetch(`/api/BuyerAd/${route.params.id}/offers`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        price: offerForm.value.price,
        deadline: deadlineDate.value.toDate(getLocalTimeZone()).toISOString(),
        description: offerForm.value.description
      }),
      credentials: "include",
    });

    if (res.ok) {
      showAlert("Sukces!", "Oferta została wysłana!", "success");
      await loadMyOffer();
      offerForm.value = { price: 0, description: "" };
      deadlineDate.value = null;
    } else {
      const text = await res.text();
      showAlert("Błąd", `Nie udało się wysłać oferty: ${text}`, "destructive");
    }
  } catch (e) {
    showAlert("Błąd sieci", "Wystąpił problem przy wysyłaniu oferty.", "destructive");
  } finally {
    submitting.value = false;
  }
}

async function onChatClick() {
  if (!ad.value?.buyerUserId) return;
  isStartingChat.value = true;
  try {
    const conversationId = await chat.getOrCreateConversation(ad.value.buyerUserId);
    if (conversationId) {
      router.push(`/chat/${conversationId}`);
    } else {
      showAlert("Błąd", "Nie udało się rozpocząć konwersacji.", "destructive");
    }
  } finally {
    isStartingChat.value = false;
  }
}

onMounted(load);
</script>

<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20 min-h-screen">
    <Container>
      <div class="mt-8 lg:mt-12 w-full flex flex-col gap-8 max-w-5xl mx-auto">
        
        <div v-if="loading" class="flex justify-center py-32">
          <Loader2 class="h-12 w-12 text-teal-600 animate-spin" />
        </div>

        <div v-else-if="error" class="bg-red-50 dark:bg-red-900/10 text-red-600 dark:text-red-400 p-6 rounded-2xl font-medium border border-red-200 dark:border-red-900/30 text-center">
          {{ error }}
        </div>

        <template v-else-if="ad">
          <!-- Main Header Card -->
          <Card class="w-full shadow-lg shadow-teal-900/5 border-zinc-200 dark:border-zinc-800 overflow-hidden relative bg-white dark:bg-zinc-900">
            <CardContent class="p-8 md:p-10 flex flex-col gap-8">
              
              <!-- Title & Meta -->
              <div class="flex flex-col md:flex-row justify-between items-start gap-6">
                <div class="flex-1">
                   <div class="flex items-center gap-3 mb-4">
                     <span class="bg-teal-50 dark:bg-teal-900/30 text-teal-700 dark:text-teal-400 font-bold px-3 py-1 rounded-full text-sm flex items-center gap-1.5 border border-teal-100 dark:border-teal-900/50">
                       <Tag class="h-3.5 w-3.5" /> {{ ad.category }}
                     </span>
                     <span v-if="ad.isClosed" class="bg-zinc-100 dark:bg-zinc-800 text-zinc-500 font-bold px-3 py-1 rounded-full text-sm flex items-center gap-1.5 border border-zinc-200 dark:border-zinc-700">
                       Zlecenie zamknięte
                     </span>
                   </div>
                   
                   <h1 class="text-3xl md:text-5xl font-black text-zinc-900 dark:text-zinc-50 leading-tight tracking-tight">
                     {{ ad.title }}
                   </h1>
                   
                   <p class="text-zinc-500 dark:text-zinc-400 mt-3 font-medium flex items-center gap-2">
                     <Clock class="h-4 w-4" /> 
                     Opublikowano {{ String(ad.createDate).slice(0, 10).replace(/-/g, '.') }}
                   </p>
                </div>
                
                <div class="flex flex-col items-end text-right bg-zinc-50 dark:bg-zinc-950 p-5 rounded-2xl border border-zinc-200 dark:border-zinc-800 shrink-0 min-w-[200px]">
                   <span class="text-xs font-bold uppercase tracking-wider text-zinc-500 dark:text-zinc-400 mb-1">Budżet projektu</span>
                   <span class="text-3xl font-black text-teal-600 dark:text-teal-400">{{ ad.budget }} zł</span>
                </div>
              </div>

              <!-- Content -->
              <div class="prose dark:prose-invert max-w-none text-zinc-700 dark:text-zinc-300">
                <p class="whitespace-pre-line text-lg leading-relaxed pt-2">
                  {{ ad.description }}
                </p>
              </div>

              <!-- Stats Grid -->
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 pt-4">
                
                <div class="flex items-center gap-4 bg-zinc-50 dark:bg-zinc-950 p-4 rounded-2xl border border-zinc-100 dark:border-zinc-800/80 hover:border-teal-200 transition-colors">
                  <div class="h-12 w-12 bg-white dark:bg-zinc-900 rounded-xl flex items-center justify-center border border-zinc-200 dark:border-zinc-800 shadow-sm text-teal-600">
                    <CalendarIcon class="h-6 w-6" />
                  </div>
                  <div class="flex flex-col text-left">
                    <span class="text-[10px] font-bold uppercase tracking-widest text-zinc-500">Termin wykonania</span>
                    <span class="text-lg font-bold text-zinc-900 dark:text-zinc-100">{{ String(ad.deadline).slice(0, 10) }}</span>
                  </div>
                </div>

                <div v-if="ad.buyerName" class="flex items-center justify-between gap-4 bg-zinc-50 dark:bg-zinc-950 p-4 rounded-2xl border border-zinc-100 dark:border-zinc-800/80 hover:border-teal-200 transition-colors">
                  <div class="flex items-center gap-4">
                    <Avatar class="h-12 w-12 border border-zinc-200 dark:border-zinc-700 shadow-sm bg-white dark:bg-zinc-900">
                      <AvatarFallback class="text-teal-700 font-bold">{{ ad.buyerName.charAt(0).toUpperCase() }}</AvatarFallback>
                    </Avatar>
                    <div class="flex flex-col text-left">
                      <span class="text-[10px] font-bold uppercase tracking-widest text-zinc-500">Zleceniodawca</span>
                      <span class="text-lg font-bold text-zinc-900 dark:text-zinc-100">{{ ad.buyerName }}</span>
                    </div>
                  </div>
                  <Button v-if="!isOwner" variant="ghost" size="icon" class="text-teal-600 hover:bg-teal-50 dark:hover:bg-teal-900/30 rounded-xl" @click="onChatClick" :disabled="isStartingChat">
                     <Loader2 v-if="isStartingChat" class="h-5 w-5 animate-spin" />
                     <MessageCircle v-else class="h-5 w-5" />
                  </Button>
                </div>
                
              </div>

              <div v-if="isOwner" class="pt-6 border-t border-zinc-100 dark:border-zinc-800 flex justify-end">
                <Button @click="router.push(`/request/${ad.id}/review`)" class="bg-zinc-900 hover:bg-zinc-800 text-white dark:bg-zinc-100 dark:hover:bg-white dark:text-zinc-900 font-bold py-6 px-8 rounded-xl shadow-md transition-all flex items-center gap-2">
                  <MessageSquare class="h-5 w-5" /> Przeglądaj oferty
                </Button>
              </div>

            </CardContent>
          </Card>

          <!-- Seller Bidding Section -->
          <div v-if="isSeller && !isOwner" class="mt-4 animate-in slide-in-from-bottom-4 duration-500">
            <Card v-if="myOffer" class="border-teal-200 dark:border-teal-900/30 bg-teal-50/50 dark:bg-teal-900/10 shadow-lg">
              <CardContent class="p-8">
                <div class="flex items-center gap-3 mb-6 relative">
                  <div class="bg-teal-100 dark:bg-teal-900/50 p-2 rounded-full absolute -left-2 -top-2">
                     <CheckCircle2 class="h-6 w-6 text-teal-600 dark:text-teal-400" />
                  </div>
                  <h3 class="text-xl font-bold text-teal-800 dark:text-teal-300 ml-10">Twoja oferta została złożona</h3>
                </div>
                
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-6">
                  <div class="p-5 bg-white dark:bg-zinc-900 rounded-2xl border border-teal-100 dark:border-teal-900/30 shadow-sm">
                    <span class="text-xs font-bold text-zinc-500 uppercase tracking-wider block mb-1">Zaproponowana cena</span>
                    <span class="text-3xl font-black text-zinc-900 dark:text-zinc-100">{{ myOffer.price }} zł</span>
                  </div>
                  <div class="p-5 bg-white dark:bg-zinc-900 rounded-2xl border border-teal-100 dark:border-teal-900/30 shadow-sm">
                    <span class="text-xs font-bold text-zinc-500 uppercase tracking-wider block mb-1">Zaproponowany termin</span>
                    <span class="text-xl font-bold text-zinc-900 dark:text-zinc-100 pt-1 block">{{ String(myOffer.deadline).slice(0, 10) }}</span>
                  </div>
                </div>
                
                <div class="p-6 bg-white dark:bg-zinc-900 rounded-2xl border border-teal-100 dark:border-teal-900/30 shadow-sm">
                   <span class="text-xs font-bold text-zinc-500 uppercase tracking-wider block mb-3">Szczegóły propozycji</span>
                   <p class="text-zinc-700 dark:text-zinc-300 leading-relaxed">{{ myOffer.description }}</p>
                </div>
              </CardContent>
            </Card>

            <Card v-else-if="!ad.isClosed" class="border-zinc-200 dark:border-zinc-800 shadow-xl overflow-hidden bg-white dark:bg-zinc-900">
              <div class="h-1.5 w-full bg-teal-500"></div>
              <CardContent class="p-8">
                <div class="mb-8 cursor-default">
                  <h3 class="text-2xl font-bold text-zinc-900 dark:text-zinc-100">Przedstaw swoją ofertę</h3>
                  <p class="text-zinc-500 dark:text-zinc-400 mt-1 font-medium">Zaproponuj klientowi swoje warunki wykonania zlecenia.</p>
                </div>
                
                <div class="flex flex-col gap-8">
                  <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div class="flex flex-col gap-2.5">
                      <label class="text-sm font-bold tracking-wide text-zinc-800 dark:text-zinc-200 uppercase">Cena realizacji (zł)</label>
                      <div class="relative">
                        <Input v-model.number="offerForm.price" type="number" placeholder="Np. 500" class="pl-12 h-14 text-lg bg-zinc-50 dark:bg-zinc-950 border-zinc-200 dark:border-zinc-800 rounded-xl focus-visible:ring-teal-500" />
                        <DollarSign class="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-zinc-400" />
                      </div>
                    </div>
                    
                    <div class="flex flex-col gap-2.5">
                      <label class="text-sm font-bold tracking-wide text-zinc-800 dark:text-zinc-200 uppercase">Proponowany termin</label>
                      <Popover>
                        <PopoverTrigger as-child>
                          <Button
                            variant="outline"
                            :class="cn('w-full h-14 justify-start text-left font-normal text-lg bg-zinc-50 border-zinc-200 dark:bg-zinc-950 dark:border-zinc-800 dark:text-zinc-300 rounded-xl hover:bg-zinc-100 dark:hover:bg-zinc-900', !deadlineDate && 'text-zinc-500')"
                          >
                            <CalendarIcon class="mr-3 h-5 w-5" />
                            {{ deadlineDate ? df.format(deadlineDate.toDate(getLocalTimeZone())) : "Wybierz datę..." }}
                          </Button>
                        </PopoverTrigger>
                        <PopoverContent class="w-auto p-0 rounded-xl" align="start">
                          <Calendar v-model="deadlineDate" initial-focus />
                        </PopoverContent>
                      </Popover>
                    </div>
                  </div>
                  
                  <div class="flex flex-col gap-2.5">
                    <label class="text-sm font-bold tracking-wide text-zinc-800 dark:text-zinc-200 uppercase">Wiadomość do klienta</label>
                    <Textarea v-model="offerForm.description" placeholder="Napisz dlaczego to właśnie Ty powinieneś otrzymać to zlecenie. Przedstaw swoje doświadczenie i pomysł..." class="min-h-[160px] text-base p-4 bg-zinc-50 dark:bg-zinc-950 border-zinc-200 dark:border-zinc-800 rounded-xl focus-visible:ring-teal-500 resize-none" />
                  </div>
                  
                  <div class="flex justify-end pt-2">
                    <Button @click="submitOffer" :disabled="submitting" class="bg-teal-600 hover:bg-teal-700 text-white h-14 px-10 text-lg font-bold rounded-xl shadow-lg shadow-teal-900/20 transition-all hover:scale-[1.02] active:scale-[0.98] w-full sm:w-auto">
                      <Loader2 v-if="submitting" class="mr-2 h-5 w-5 animate-spin" />
                      <Send v-else class="mr-2 h-5 w-5" />
                      Wyślij Propozycję
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </template>

      </div>
    </Container>
  </div>
</template>
