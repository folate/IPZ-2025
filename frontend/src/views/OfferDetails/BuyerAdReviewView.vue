<script setup>
import { ref, onMounted, computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import Container from "@/components/ui/Container.vue";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Loader2, Calendar, DollarSign, User, MessageSquare, CheckCircle, ArrowLeft } from "lucide-vue-next";
import { useChat } from "@/stores/chat";
import { useAlert } from "@/stores/alert";

const route = useRoute();
const router = useRouter();
const chat = useChat();
const { showAlert } = useAlert();

const loading = ref(false);
const accepting = ref(null); // ID of offer being accepted
const isStartingChat = ref(null); // ID of offer for which chat is starting
const error = ref("");
const offers = ref([]);
const ad = ref(null);

const hasAcceptedOffer = computed(() => offers.value.some(o => o.isAccepted));

async function load() {
  loading.value = true;
  error.value = "";
  try {
    // Load Ad details
    const adRes = await fetch(`/api/BuyerAd/${route.params.id}`, { credentials: "include" });
    if (adRes.ok) ad.value = await adRes.json();

    // Load Offers
    const res = await fetch(`/api/BuyerAd/${route.params.id}/offers`, { credentials: "include" });
    if (res.ok) {
      offers.value = await res.json();
    } else {
      error.value = "Nie udało się pobrać ofert.";
    }
  } catch {
    error.value = "Błąd sieci.";
  } finally {
    loading.value = false;
  }
}

async function acceptOffer(offerId) {
  if (!confirm("Czy na pewno chcesz zaakceptować tę ofertę? Spowoduje to zamknięcie zlecenia.")) return;

  accepting.value = offerId;
  try {
    const res = await fetch(`/api/BuyerAd/offers/${offerId}/accept`, {
      method: "POST",
      credentials: "include",
    });

    if (res.ok) {
      alert("Oferta zaakceptowana! Utworzono nowe zamówienie.");
      router.push(`/request/${route.params.id}`);
    } else {
      const text = await res.text();
      showAlert("Błąd", `Wystąpił błąd: ${text}`, "destructive");
    }
  } catch {
    showAlert("Błąd", "Wystąpił błąd sieci.", "destructive");
  } finally {
    accepting.value = null;
  }
}

async function startChat(offer) {
  const targetId = offer.freelancerUserId || offer.freelancer?.userId;
  if (!targetId) {
     showAlert("Błąd", "Nie można zidentyfikować wykonawcy.", "destructive");
     return;
  }
  
  isStartingChat.value = offer.id;
  try {
    const conversationId = await chat.getOrCreateConversation(targetId);
    if (conversationId) {
      router.push(`/chat/${conversationId}`);
    } else {
      showAlert("Błąd", "Nie udało się rozpocząć konwersacji.", "destructive");
    }
  } catch (e) {
    showAlert("Błąd", "Wystąpił błąd podczas próby otwarcia chatu.", "destructive");
  } finally {
    isStartingChat.value = null;
  }
}

onMounted(load);
</script>

<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20 min-h-screen">
    <Container>
      <div class="mt-8 flex flex-col gap-6">
        
        <div class="flex items-center justify-between">
          <Button variant="ghost" @click="router.back()" class="flex items-center gap-2">
            <ArrowLeft class="h-4 w-4" /> Powrót
          </Button>
          <h1 class="text-2xl font-black text-zinc-900 dark:text-zinc-50">Przegląd ofert</h1>
        </div>

        <div v-if="loading" class="flex justify-center py-20">
          <Loader2 class="h-10 w-10 text-teal-600 animate-spin" />
        </div>

        <div v-else-if="error" class="bg-red-50 text-red-600 p-6 rounded-2xl border border-red-200">
          {{ error }}
        </div>

        <div v-else-if="offers.length === 0" class="text-center py-20 bg-white dark:bg-zinc-900 rounded-2xl border border-zinc-200 dark:border-zinc-800">
           <p class="text-zinc-500 text-lg">Brak złożonych ofert dla tego zlecenia.</p>
        </div>

        <div v-else class="grid grid-cols-1 gap-6">
          <Card v-for="offer in offers" :key="offer.id" class="border-zinc-200 dark:border-zinc-800 shadow-lg hover:shadow-xl transition-shadow overflow-hidden">
            <CardHeader class="pb-2">
              <div class="flex justify-between items-start">
                <div class="flex items-center gap-3">
                  <div class="h-12 w-12 bg-zinc-100 dark:bg-zinc-800 rounded-full flex items-center justify-center">
                    <User class="h-6 w-6 text-zinc-500" />
                  </div>
                  <div>
                    <CardTitle class="text-xl font-bold">{{ offer.freelancerName }}</CardTitle>
                    <CardDescription>Złożono {{ new Date(offer.createdAt).toLocaleDateString() }}</CardDescription>
                  </div>
                </div>
                <div class="text-right">
                  <p class="text-2xl font-black text-teal-600">{{ offer.price }} zł</p>
                  <p class="text-xs font-bold text-zinc-400 uppercase tracking-widest flex items-center justify-end gap-1">
                    <Calendar class="h-3 w-3" /> {{ String(offer.deadline).slice(0,10) }}
                  </p>
                </div>
              </div>
            </CardHeader>
            <CardContent class="flex flex-col gap-6">
              <div class="bg-zinc-50 dark:bg-zinc-900/50 p-4 rounded-xl border border-zinc-100 dark:border-zinc-800">
                <p class="text-zinc-700 dark:text-zinc-300 whitespace-pre-line">{{ offer.description }}</p>
              </div>

              <div class="flex justify-end gap-3">
                <Button 
                   variant="outline" 
                   class="flex items-center gap-2 font-bold border-zinc-300"
                   @click="startChat(offer)"
                   :disabled="isStartingChat === offer.id"
                >
                  <Loader2 v-if="isStartingChat === offer.id" class="h-4 w-4 animate-spin" />
                  <MessageSquare v-else class="h-4 w-4" /> Wiadomość
                </Button>
                
                <Button 
                  v-if="!ad?.isClosed && !hasAcceptedOffer"
                  @click="acceptOffer(offer.id)" 
                  :disabled="accepting !== null"
                  class="bg-teal-600 hover:bg-teal-700 text-white font-bold px-6 flex items-center gap-2"
                >
                  <Loader2 v-if="accepting === offer.id" class="h-4 w-4 animate-spin" />
                  <CheckCircle v-else class="h-4 w-4" />
                  Akceptuj ofertę
                </Button>
                <div v-else-if="offer.isAccepted" class="bg-green-100 text-green-700 px-4 py-2 rounded-lg font-bold flex items-center gap-2">
                  <CheckCircle class="h-4 w-4" /> Oferta zaakceptowana
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

      </div>
    </Container>
  </div>
</template>
