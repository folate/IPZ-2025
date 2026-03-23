<script setup>
import { ref, onMounted, computed } from "vue"
import { useRoute, useRouter } from "vue-router"
import { useAuth } from "@/stores/auth"

import LandingHeader from "@/components/landing/LandingHeader.vue"
import Container from "@/components/ui/Container.vue"

import { Card, CardContent, CardHeader, CardTitle, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"

import { Package, Clock, CheckCircle2, RotateCcw, MessageSquare, AlertCircle, Send, Check, Loader2, Upload } from "lucide-vue-next"

const route = useRoute()
const router = useRouter()
const { isLoggedIn, initAuth, hasRole } = useAuth()

const pageLoading = ref(true)

// Placeholder Data
const orderInfo = ref({
  id: route.params.id || "ORD-001",
  title: "Nowoczesna strona internetowa WordPress",
  seller: "WebDevPro",
  price: "1500 zł",
  deadline: "2026-03-20",
  status: "delivered" // active, delivered, revision, completed
})

const historyEvents = ref([
  {
    id: 1,
    type: "order_started",
    date: "2026-03-10 09:00",
    title: "Zamówienie rozpoczęte",
    message: "Sprzedawca rozpoczął pracę nad Twoim zamówieniem. Oczekiwany termin dostawy: 2026-03-20."
  },
  {
    id: 2,
    type: "delivery",
    date: "2026-03-13 14:30",
    title: "Dostarczono pracę",
    message: "Cześć! Przesyłam pierwszą wersję strony. Wykorzystałem nowoczesny motyw i spełniłem wszystkie założenia z briefu. Proszę o recenzję i ewentualne uwagi, pozdrawiam!",
    files: ["v1_preview_link.txt", "architektura.pdf"]
  },
  {
    id: 3,
    type: "revision_requested",
    date: "2026-03-13 18:45",
    title: "Prośba o poprawkę",
    message: "Wygląda świetnie, ale czy moglibyśmy zmienić kolorystykę nagłówka na nieco ciemniejszą? Ten błękit trochę za bardzo razi w oczy."
  }
])

const isSeller = computed(() => hasRole('SELLER'))
const revisionMessage = ref("")
const deliveryMessage = ref("")
const isSubmitting = ref(false)

const requestRevision = () => {
  if (!revisionMessage.value.trim()) return
  
  isSubmitting.value = true
  setTimeout(() => {
    historyEvents.value.push({
      id: Date.now(),
      type: "revision_requested",
      date: new Date().toISOString().replace('T', ' ').slice(0, 16),
      title: "Prośba o poprawkę",
      message: revisionMessage.value
    })
    orderInfo.value.status = "revision"
    revisionMessage.value = ""
    isSubmitting.value = false
  }, 1000)
}

const acceptDelivery = () => {
  isSubmitting.value = true
  setTimeout(() => {
    historyEvents.value.push({
      id: Date.now(),
      type: "completed",
      date: new Date().toISOString().replace('T', ' ').slice(0, 16),
      title: "Zamówienie zakończone",
      message: "Dostawa została zaakceptowana. Zamówienie jest uznane za zakończone."
    })
    orderInfo.value.status = "completed"
    isSubmitting.value = false
    alert("Dostawa została zaakceptowana!")
  }, 1000)
}

const deliverWork = () => {
  if (!deliveryMessage.value.trim()) return

  isSubmitting.value = true
  setTimeout(() => {
    historyEvents.value.push({
      id: Date.now(),
      type: "delivery",
      date: new Date().toISOString().replace('T', ' ').slice(0, 16),
      title: "Dostarczono pracę",
      message: deliveryMessage.value,
      files: ["plik_koncowy.zip"]
    })
    orderInfo.value.status = "delivered"
    deliveryMessage.value = ""
    isSubmitting.value = false
    alert("Praca została wysłana do kupującego!")
  }, 1000)
}

onMounted(async () => {
  await initAuth()
  if (!isLoggedIn.value) {
    router.push("/login")
    return
  }
  pageLoading.value = false
})

</script>

<template>
  <div class="min-h-svh bg-zinc-50 dark:bg-zinc-950 pb-20">
    <LandingHeader />

    <Container>
      
      <div v-if="pageLoading" class="mt-20 flex flex-col items-center justify-center">
        <Loader2 class="h-10 w-10 animate-spin text-teal-600 mb-4" />
        <p class="text-zinc-500 font-medium">Sprawdzanie autoryzacji...</p>
      </div>

      <div v-else class="mt-8">
        
        <!-- Header -->
        <div class="flex flex-col md:flex-row md:items-end justify-between mb-8 gap-4">
          <div>
            <div class="flex items-center gap-3 mb-2">
              <h1 class="text-3xl font-extrabold text-zinc-900 dark:text-zinc-50">Zamówienie #{{ orderInfo.id }}</h1>
              <Badge v-if="orderInfo.status === 'delivered'" class="bg-blue-100 text-blue-700 dark:bg-blue-900/40 dark:text-blue-400 hover:bg-blue-100 px-3 py-1">Oczekuje na recenzję</Badge>
              <Badge v-else-if="orderInfo.status === 'completed'" class="bg-teal-100 text-teal-700 dark:bg-teal-900/40 dark:text-teal-400 hover:bg-teal-100 px-3 py-1">Zakończone</Badge>
              <Badge v-else-if="orderInfo.status === 'revision'" class="bg-amber-100 text-amber-700 dark:bg-amber-900/40 dark:text-amber-400 hover:bg-amber-100 px-3 py-1">W poprawie</Badge>
              <Badge v-else class="bg-zinc-100 text-zinc-700 dark:bg-zinc-800 dark:text-zinc-400 hover:bg-zinc-100 px-3 py-1">W trakcie</Badge>
            </div>
            <p class="text-zinc-500 dark:text-zinc-400 text-lg">{{ orderInfo.title }} (Od: <span class="font-medium text-zinc-700 dark:text-zinc-300">{{ orderInfo.seller }}</span>)</p>
          </div>
          
          <div class="flex gap-4 items-center bg-white dark:bg-zinc-900 px-4 py-2 rounded-xl border border-zinc-200 dark:border-zinc-800 shadow-sm">
             <div class="flex flex-col">
                <span class="text-xs text-zinc-400 uppercase font-bold">Wartość</span>
                <span class="font-bold text-zinc-900 dark:text-white">{{ orderInfo.price }}</span>
             </div>
             <div class="w-px h-8 bg-zinc-200 dark:bg-zinc-800 mx-2"></div>
             <div class="flex flex-col">
               <span class="text-xs text-zinc-400 uppercase font-bold">Deadline</span>
               <span class="font-bold text-zinc-900 dark:text-white">{{ orderInfo.deadline }}</span>
             </div>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          
          <!-- Activity Timeline (Left/Main Column) -->
          <div class="lg:col-span-2 flex flex-col gap-6">
            
            <h2 class="text-xl font-bold text-zinc-900 dark:text-zinc-50 mb-2">Aktywność Zamówienia</h2>
            
            <div class="flex flex-col gap-8 relative before:absolute before:inset-0 before:ml-6 before:-translate-x-px md:before:mx-auto md:before:translate-x-0 before:h-full before:w-0.5 before:bg-gradient-to-b before:from-transparent before:via-zinc-200 dark:before:via-zinc-800 before:to-transparent">
              
              <div v-for="event in historyEvents" :key="event.id" class="relative flex items-center justify-between md:justify-normal md:odd:flex-row-reverse group is-active">
                
                <!-- Icon -->
                <div class="flex items-center justify-center w-12 h-12 rounded-full border-4 border-zinc-50 dark:border-zinc-950 shrink-0 md:order-1 md:group-odd:-translate-x-1/2 md:group-even:translate-x-1/2 shadow-sm z-10"
                  :class="{
                    'bg-slate-100 text-slate-500 dark:bg-slate-900 dark:text-slate-400': event.type === 'order_started',
                    'bg-teal-100 text-teal-600 dark:bg-teal-900/60 dark:text-teal-400': event.type === 'delivery',
                    'bg-amber-100 text-amber-600 dark:bg-amber-900/60 dark:text-amber-400': event.type === 'revision_requested',
                    'bg-teal-500 text-white': event.type === 'completed',
                  }"
                >
                  <Package v-if="event.type === 'order_started'" class="w-5 h-5" />
                  <CheckCircle2 v-else-if="event.type === 'delivery'" class="w-5 h-5" />
                  <RotateCcw v-else-if="event.type === 'revision_requested'" class="w-5 h-5" />
                  <Check v-else-if="event.type === 'completed'" class="w-5 h-5" />
                  <MessageSquare v-else class="w-5 h-5" />
                </div>

                <!-- Content Card -->
                <div class="w-[calc(100%-4rem)] md:w-[calc(50%-3rem)] p-4 rounded-2xl bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 shadow-sm">
                  <div class="flex items-center justify-between mb-2">
                    <h3 class="font-bold text-zinc-900 dark:text-zinc-100">{{ event.title }}</h3>
                    <time class="text-xs text-zinc-400 font-medium">{{ event.date }}</time>
                  </div>
                  <p class="text-sm text-zinc-600 dark:text-zinc-400 mb-4 whitespace-pre-wrap">{{ event.message }}</p>
                  
                  <div v-if="event.files && event.files.length" class="flex flex-wrap gap-2">
                    <div v-for="file in event.files" :key="file" class="flex items-center gap-2 bg-zinc-100 dark:bg-zinc-950 border border-zinc-200 dark:border-zinc-800 rounded-lg px-3 py-1.5 text-xs text-zinc-600 dark:text-zinc-300 font-medium">
                      <Package class="w-3.5 h-3.5 text-teal-500" />
                      {{ file }}
                    </div>
                  </div>
                </div>

              </div>

            </div>
          </div>

          <!-- Right Column: Action Box -->
          <div class="flex flex-col gap-6">
            <h2 class="text-xl font-bold text-zinc-900 dark:text-zinc-50 mb-2">Decyzja Kupującego</h2>

            <!-- Buyer View: Delivery Decision Card -->
            <Card v-if="orderInfo.status === 'delivered' && !isSeller" class="border-teal-200 dark:border-teal-900/50 shadow-xl shadow-teal-900/5 bg-white dark:bg-zinc-900 relative overflow-hidden">
               <div class="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-teal-400 to-emerald-500"></div>
               <CardHeader class="pb-3">
                 <CardTitle class="flex items-center gap-2 text-xl text-teal-800 dark:text-teal-400">
                   <AlertCircle class="w-5 h-5" /> Akcja Wymagana
                 </CardTitle>
               </CardHeader>
               <CardContent class="flex flex-col gap-6">
                 <p class="text-sm text-zinc-600 dark:text-zinc-400 font-medium">Sprzedawca dostarczył pracę. Sprawdź pliki w historii po lewej stronie. Jeśli wszystko jest w porządku, zaakceptuj zamówienie. Możesz też poprosić o poprawkę (rewizję).</p>
                 
                 <div class="flex flex-col gap-3">
                   <Button @click="acceptDelivery" :disabled="isSubmitting" class="w-full h-12 bg-teal-600 hover:bg-teal-700 text-white font-bold text-base shadow-md">
                     <CheckCircle2 class="w-5 h-5 mr-2" /> Zaakceptuj Dostawę
                   </Button>
                 </div>

                 <div class="border-t border-zinc-100 dark:border-zinc-800 pt-6">
                   <Label class="text-sm font-bold text-zinc-700 dark:text-zinc-300 mb-2 block">Potrzebujesz poprawek?</Label>
                   <Textarea v-model="revisionMessage" placeholder="Opisz dokładnie co sprzedawca powinien zmienić lub poprawić..." class="min-h-[120px] mb-3 bg-zinc-50 dark:bg-zinc-950 border-zinc-200 dark:border-zinc-800" />
                   <Button @click="requestRevision" :disabled="isSubmitting || !revisionMessage.trim()" variant="outline" class="w-full h-11 border-amber-200 text-amber-700 hover:bg-amber-50 dark:border-amber-900/50 dark:text-amber-400 dark:hover:bg-amber-900/20">
                     <RotateCcw class="w-4 h-4 mr-2" /> Poproś o Rewizję
                   </Button>
                 </div>
               </CardContent>
            </Card>

            <!-- Seller View: Deliver Work Card -->
            <Card v-else-if="(orderInfo.status === 'revision' || orderInfo.status === 'active' || orderInfo.status === 'delivered') && isSeller" class="border-indigo-200 dark:border-indigo-900/50 shadow-xl shadow-teal-900/5 bg-white dark:bg-zinc-900 relative overflow-hidden">
               <div class="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-indigo-400 to-indigo-600"></div>
               <CardHeader class="pb-3">
                 <CardTitle class="flex items-center gap-2 text-xl text-indigo-800 dark:text-indigo-400">
                   <Upload class="w-5 h-5" /> Dostarcz Pracę
                 </CardTitle>
               </CardHeader>
               <CardContent class="flex flex-col gap-6">
                 <p v-if="orderInfo.status === 'delivered'" class="text-sm text-amber-600 dark:text-amber-400 font-medium">Oczekujesz na weryfikację dostarczonej pracy przez kupującego, ale możesz w międzyczasie przesłać nową wersję.</p>
                 <p v-else-if="orderInfo.status === 'revision'" class="text-sm text-zinc-600 dark:text-zinc-400 font-medium">Kupujący poprosił o poprawkę. Popraw pracę uwzględniając jego uwagi z osi czasu i prześlij zaktualizowaną wersję projektu.</p>
                 <p v-else class="text-sm text-zinc-600 dark:text-zinc-400 font-medium">Wyślij kupującemu gotową pracę. Zostanie on powiadomiony e-mailem i będzie miał czas na jej akceptację.</p>
                 
                 <div class="flex flex-col gap-4">
                   <div class="flex flex-col gap-2">
                     <Label class="text-sm font-bold text-zinc-700 dark:text-zinc-300">Wiadomość do dostawy</Label>
                     <Textarea v-model="deliveryMessage" placeholder="Opisz wysyłane pliki lub to co zmieniłeś..." class="min-h-[120px] bg-zinc-50 dark:bg-zinc-950 border-zinc-200 dark:border-zinc-800" />
                   </div>
                   
                   <!-- Symulacja dodawania pliku -->
                   <div class="border-2 border-dashed border-zinc-200 dark:border-zinc-800 rounded-xl p-6 flex flex-col items-center justify-center text-zinc-500 hover:bg-zinc-50 dark:hover:bg-zinc-900/50 cursor-pointer transition-colors">
                     <Upload class="w-8 h-8 mb-2 opacity-50 text-indigo-600" />
                     <span class="font-medium text-sm">Kliknij, aby załączyć plik (Max 5GB)</span>
                   </div>

                   <Button @click="deliverWork" :disabled="isSubmitting || !deliveryMessage.trim()" class="w-full h-12 bg-indigo-600 hover:bg-indigo-700 text-white font-bold text-base shadow-md mt-2">
                     <Send class="w-5 h-5 mr-2" /> Wyślij Dostawę
                   </Button>
                 </div>
               </CardContent>
            </Card>

            <Card v-else-if="orderInfo.status === 'completed'" class="border-zinc-200 dark:border-zinc-800 bg-zinc-50 dark:bg-zinc-900">
              <CardContent class="pt-6 flex flex-col items-center justify-center text-center p-8 gap-4">
                <div class="w-16 h-16 rounded-full bg-teal-100 dark:bg-teal-900/40 flex justify-center items-center">
                  <Check class="w-8 h-8 text-teal-600 dark:text-teal-400" />
                </div>
                <div>
                  <h3 class="font-bold text-xl text-zinc-900 dark:text-zinc-100">Zamówienie Zakończone</h3>
                  <p class="text-zinc-500 dark:text-zinc-400 text-sm mt-2">Dziękujemy za współpracę. Dostawa została pomyślnie zaakceptowana.</p>
                </div>
              </CardContent>
            </Card>

            <Card v-else-if="orderInfo.status === 'revision' && !isSeller" class="border-amber-200 dark:border-amber-900/50 bg-amber-50/50 dark:bg-amber-900/10">
              <CardContent class="pt-6 flex flex-col items-center justify-center text-center p-8 gap-4">
                <div class="w-16 h-16 rounded-full bg-amber-100 dark:bg-amber-900/40 flex justify-center items-center">
                  <Clock class="w-8 h-8 text-amber-600 dark:text-amber-400" />
                </div>
                <div>
                  <h3 class="font-bold text-xl text-amber-900 dark:text-amber-400">Sprzedawca pracuje</h3>
                  <p class="text-amber-700/80 dark:text-amber-400/80 text-sm mt-2">Przesłałeś prośbę o poprawkę. Twój freelancer obecnie przygotowuje nową wersję plików do weryfikacji.</p>
                </div>
              </CardContent>
            </Card>

          </div>
        </div>

      </div>
    </Container>
  </div>
</template>
