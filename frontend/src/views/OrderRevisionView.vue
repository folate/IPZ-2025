<script setup>
import { ref, onMounted, computed } from "vue"
import { useRoute, useRouter } from "vue-router"
import { useAuth } from "@/stores/auth"
import { useAlert } from "@/stores/alert"

import Container from "@/components/ui/Container.vue"

import { Card, CardContent, CardHeader, CardTitle, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"

import { Package, Clock, CheckCircle2, RotateCcw, MessageSquare, AlertCircle, Send, Check, Loader2, Upload } from "lucide-vue-next"

const route = useRoute()
const router = useRouter()
const { isLoggedIn, initAuth, hasRole, state: authState } = useAuth()
const { showAlert } = useAlert()

const pageLoading = ref(true)

const orderIdStr = route.params.id || "ORD-001"
const numericId = parseInt(orderIdStr.replace(/\D/g, ''), 10) || 1

const orderInfo = ref({
  id: orderIdStr,
  title: "Zamówienie (Pobieranie...)",
  seller: "Sprzedawca / Kupujący",
  price: "-",
  deadline: "-",
  status: "active",
  additionalInstructions: "",
  gigDescription: ""
})

const currentUser = computed(() => authState.user)
const currentUserId = computed(() => authState.user?.id)

const historyEvents = ref([
  {
    id: 1,
    type: "order_started",
    date: new Date().toISOString().replace('T', ' ').slice(0, 16),
    title: "Zamówienie rozpoczęte",
    message: "Rozpoczęto pracę nad Twoim zamówieniem."
  }
])

const isSeller = computed(() => hasRole('SELLER'))
const revisionMessage = ref("")
const deliveryMessage = ref("")
const selectedFiles = ref([])
const isSubmitting = ref(false)

const handleFileChange = (e) => {
  selectedFiles.value = Array.from(e.target.files)
}

const downloadFile = (fileId, fileName) => {
  window.open(`/api/Revision/download/${fileId}`, '_blank')
}

const fetchOrderDetails = async () => {
  try {
    const res = await fetch(`/api/Order/get/${numericId}`, { credentials: "include" })
    if (res.ok) {
      const data = await res.json()
      orderInfo.value = {
        ...orderInfo.value,
        id: data.id,
        extOrderId: data.extOrderId,
        title: data.gig.title,
        seller: isSeller.value ? `${data.buyer.firstName} ${data.buyer.lastName}` : `${data.seller.firstName} ${data.seller.lastName}`,
        price: `${data.price} PLN`,
        deadline: new Date(data.orderDate).toLocaleDateString(),
        status: data.status.toLowerCase(),
        additionalInstructions: data.additionalInstructions,
        gigDescription: data.gig.description
      }
      
      // Update history event message for initial order
      if (historyEvents.value[0].type === "order_started" && data.additionalInstructions) {
         historyEvents.value[0].message = `Kupujący podał dodatkowe instrukcje: ${data.additionalInstructions}`;
      }
    }
  } catch (e) {
    console.error("Błąd pobierania szczegółów zamówienia:", e)
  }
}

const fetchRevisions = async () => {
  try {
    const res = await fetch(`/api/Revision/get?orderId=${numericId}`, { credentials: "include" })
    if (!res.ok) return;
    
    const revisions = await res.json()
    if (revisions && revisions.length > 0) {
      const newEvents = revisions.map(r => {
        let evType = "revision_requested";
        let evTitle = "Prośba o poprawkę";
        
        if (r.status === "Delivered") {
           evType = "delivery";
           evTitle = "Aktualizacja pracy";
        } else if (r.status === "Completed") {
           evType = "completed";
           evTitle = "Zaakceptowano poprawkę";
        } else if (r.status === "Pending") {
           // If it's pending but from buyer, it might be just additional info if not in 'delivered' state
           // but for simplicity we keep it as is or label as "Additional Information"
           evType = "revision_requested";
           evTitle = "Dodatkowe informacje";
        }
        
        const isFromMe = r.senderId === currentUserId.value;

        return {
          id: r.id + 1000, 
          type: evType,
          date: new Date(r.requestDate).toISOString().replace('T', ' ').slice(0, 16),
          title: evTitle,
          message: r.reason || "Brak opisu",
          files: r.files || [],
          isFromMe: isFromMe
        }
      });
      
      historyEvents.value = [
          historyEvents.value[0], 
          ...newEvents
      ];
      
      const lastRev = revisions[revisions.length - 1];
      if (lastRev.status === "Pending") orderInfo.value.status = "revision";
      else if (lastRev.status === "Delivered") orderInfo.value.status = "delivered";
      else if (lastRev.status === "Completed") orderInfo.value.status = "completed";
    }
  } catch(e) { 
    console.error("Błąd pobierania rewizji:", e) 
  }
}

const requestRevision = async () => {
  if (!revisionMessage.value.trim()) return
  
  isSubmitting.value = true
  try {
    const formData = new FormData()
    formData.append("orderId", numericId)
    formData.append("reason", revisionMessage.value)
    selectedFiles.value.forEach(file => {
      formData.append("files", file)
    })

    const res = await fetch("/api/Revision/create", {
        method: "PUT",
        body: formData
    })
    
    if (res.ok) {
        await fetchRevisions()
        orderInfo.value.status = "revision"
        revisionMessage.value = ""
        selectedFiles.value = []
    } else {
        showAlert("Błąd", "Błąd tworzenia rewizji. Serwer zwrócił błąd.", "destructive")
    }
  } catch (e) {
      console.error(e)
  }
  isSubmitting.value = false
}

const acceptDelivery = async () => {
  isSubmitting.value = true
  try {
    const formData = new FormData()
    formData.append("orderId", numericId)
    formData.append("status", "Completed")

    const res = await fetch("/api/Revision/update", {
      method: "POST",
      body: formData
    })

    if (res.ok) {
        await fetchRevisions()
        orderInfo.value.status = "completed"
        showAlert("Sukces", "Dostawa została zaakceptowana!")
    } else {
        showAlert("Błąd", "Błąd aktualizacji statusu rewizji", "destructive")
    }
  } catch(e) { console.error(e) }
  isSubmitting.value = false
}

const deliverWork = async () => {
  if (!deliveryMessage.value.trim()) return

  isSubmitting.value = true
  try {
    const formData = new FormData()
    formData.append("orderId", numericId)
    formData.append("status", "Delivered")
    formData.append("reason", deliveryMessage.value)
    selectedFiles.value.forEach(file => {
      formData.append("files", file)
    })

    const res = await fetch("/api/Revision/update", {
      method: "POST",
      body: formData
    })
    
    if (res.ok) {
        await fetchRevisions()
        orderInfo.value.status = "delivered"
        deliveryMessage.value = ""
        selectedFiles.value = []
        showAlert("Sukces", "Praca została wysłana do kupującego!")
    } else {
        showAlert("Błąd", "Błąd aktualizacji statusu.", "destructive")
    }
  } catch(e) { console.error(e) }
  isSubmitting.value = false
}

onMounted(async () => {
  await initAuth()
  if (!isLoggedIn.value) {
    router.push("/login")
    return
  }
  
  await fetchOrderDetails()
  await fetchRevisions()
  
  pageLoading.value = false
})

</script>

<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20">

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
              <Badge v-if="orderInfo.status === 'delivered' || orderInfo.status === 'paid' && orderInfo.status === 'delivered'" class="bg-blue-100 text-blue-700 dark:bg-blue-900/40 dark:text-blue-400 hover:bg-blue-100 px-3 py-1">Oczekuje na recenzję</Badge>
              <Badge v-else-if="orderInfo.status === 'completed'" class="bg-teal-100 text-teal-700 dark:bg-teal-900/40 dark:text-teal-400 hover:bg-teal-100 px-3 py-1">Zakończone</Badge>
              <Badge v-else-if="orderInfo.status === 'revision'" class="bg-amber-100 text-amber-700 dark:bg-amber-900/40 dark:text-amber-400 hover:bg-amber-100 px-3 py-1">W poprawie</Badge>
              <Badge v-else-if="orderInfo.status === 'paid' || orderInfo.status === 'active'" class="bg-zinc-100 text-zinc-700 dark:bg-zinc-800 dark:text-zinc-400 hover:bg-zinc-100 px-3 py-1">W trakcie</Badge>
              <Badge v-else class="bg-zinc-100 text-zinc-700 dark:bg-zinc-800 dark:text-zinc-400 hover:bg-zinc-100 px-3 py-1 uppercase">{{ orderInfo.status }}</Badge>
            </div>
            <p class="text-zinc-500 dark:text-zinc-400 text-lg">{{ orderInfo.title }} ({{ isSeller ? 'Dla: ' : 'Od: ' }} <span class="font-medium text-zinc-700 dark:text-zinc-300">{{ orderInfo.seller }}</span>)</p>
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
              
            <div v-for="event in historyEvents" :key="event.id" class="relative flex items-center justify-between md:justify-normal group is-active" :class="{'md:flex-row-reverse': event.isFromMe}">
              
              <!-- Icon -->
              <div class="flex items-center justify-center w-12 h-12 rounded-full border-4 border-zinc-50 dark:border-zinc-950 shrink-0 md:order-1 md:absolute md:left-1/2 md:-translate-x-1/2 shadow-sm z-10"
                :class="{
                  'bg-slate-100 text-slate-500 dark:bg-slate-900 dark:text-slate-400': event.type === 'order_started',
                  'bg-teal-100 text-teal-600 dark:bg-teal-900/60 dark:text-teal-400': event.type === 'delivery',
                  'bg-amber-100 text-amber-600 dark:bg-amber-900/60 dark:text-amber-400': event.title === 'Prośba o poprawkę',
                  'bg-blue-100 text-blue-600 dark:bg-blue-900/60 dark:text-blue-400': event.title === 'Dodatkowe informacje',
                  'bg-teal-500 text-white': event.type === 'completed',
                }"
              >
                  <Package v-if="event.type === 'order_started'" class="w-5 h-5" />
                  <CheckCircle2 v-else-if="event.type === 'delivery'" class="w-5 h-5" />
                  <RotateCcw v-else-if="event.title === 'Prośba o poprawkę'" class="w-5 h-5" />
                  <MessageSquare v-else-if="event.title === 'Dodatkowe informacje'" class="w-5 h-5" />
                  <Check v-else-if="event.type === 'completed'" class="w-5 h-5" />
                  <MessageSquare v-else class="w-5 h-5" />
                </div>

                <!-- Content Card -->
                <div class="w-[calc(100%-4rem)] md:w-[calc(50%-3rem)] p-4 rounded-2xl bg-white dark:bg-zinc-900 border shadow-sm"
                  :class="{
                    'border-teal-100 dark:border-teal-900/30': event.isFromMe && event.type !== 'revision_requested',
                    'border-amber-100 dark:border-amber-900/30': event.isFromMe && event.title === 'Prośba o poprawkę',
                    'border-blue-100 dark:border-blue-900/30': event.isFromMe && event.title === 'Dodatkowe informacje',
                    'border-zinc-200 dark:border-zinc-800': !event.isFromMe
                  }"
                >
                  <div class="flex items-center justify-between mb-2">
                    <h3 class="font-bold text-zinc-900 dark:text-zinc-100">{{ event.title }}</h3>
                    <time class="text-xs text-zinc-400 font-medium">{{ event.date }}</time>
                  </div>
                  <p class="text-sm text-zinc-600 dark:text-zinc-400 mb-4 whitespace-pre-wrap">{{ event.message }}</p>
                  
                  <div v-if="event.files && event.files.length" class="flex flex-wrap gap-2">
                    <button v-for="file in event.files" :key="file.id" @click="downloadFile(file.id, file.fileName)" class="flex items-center gap-2 bg-zinc-100 dark:bg-zinc-950 border border-zinc-200 dark:border-zinc-800 rounded-lg px-3 py-1.5 text-xs text-zinc-600 dark:text-zinc-300 font-medium hover:bg-zinc-200 dark:hover:bg-zinc-800 transition-colors">
                      <Package class="w-3.5 h-3.5 text-teal-500" />
                      {{ file.fileName }}
                    </button>
                  </div>
                </div>

              </div>

            </div>
          </div>

          <!-- Right Column: Action Box -->
          <div class="flex flex-col gap-6">
            
            <!-- Order Details Box -->
            <Card class="border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 shadow-sm">
              <CardHeader class="pb-2">
                <CardTitle class="text-sm font-bold uppercase text-zinc-400">Szczegóły usługi</CardTitle>
              </CardHeader>
              <CardContent class="flex flex-col gap-4">
                <div>
                  <h4 class="font-bold text-zinc-900 dark:text-zinc-100 leading-tight mb-1">{{ orderInfo.title }}</h4>
                  <p class="text-xs text-zinc-500 dark:text-zinc-400 line-clamp-3">{{ orderInfo.gigDescription }}</p>
                </div>
                
                <div v-if="orderInfo.additionalInstructions" class="pt-4 border-t border-zinc-100 dark:border-zinc-800">
                  <h4 class="text-xs font-bold uppercase text-zinc-400 mb-2">Instrukcje od kupującego:</h4>
                  <div class="bg-zinc-50 dark:bg-zinc-950 p-3 rounded-lg border border-zinc-200 dark:border-zinc-800 text-sm text-zinc-600 dark:text-zinc-400 italic">
                    "{{ orderInfo.additionalInstructions }}"
                  </div>
                </div>
              </CardContent>
            </Card>

            <h2 class="text-xl font-bold text-zinc-900 dark:text-zinc-50 mb-2">Akcje Zamówienia</h2>

            <!-- Buyer View: Action Card -->
            <Card v-if="!isSeller && orderInfo.status !== 'completed'" class="border-teal-200 dark:border-teal-900/50 shadow-xl shadow-teal-900/5 bg-white dark:bg-zinc-900 relative overflow-hidden">
               <div class="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-teal-400 to-emerald-500"></div>
               <CardHeader class="pb-3">
                 <CardTitle class="flex items-center gap-2 text-xl text-teal-800 dark:text-teal-400">
                   <AlertCircle class="w-5 h-5" /> {{ orderInfo.status === 'delivered' ? 'Akcja Wymagana' : 'Dodaj informacje' }}
                 </CardTitle>
               </CardHeader>
               <CardContent class="flex flex-col gap-6">
                 <p v-if="orderInfo.status === 'delivered'" class="text-sm text-zinc-600 dark:text-zinc-400 font-medium">Sprzedawca dostarczył pracę. Sprawdź pliki w historii po lewej stronie. Jeśli wszystko jest w porządku, zaakceptuj zamówienie. Możesz też poprosić o poprawkę (rewizję).</p>
                 <p v-else class="text-sm text-zinc-600 dark:text-zinc-400 font-medium">Możesz przesłać sprzedawcy dodatkowe instrukcje, uwagi lub pliki pomocnicze w dowolnym momencie trwania zamówienia.</p>
                 
                 <div v-if="orderInfo.status === 'delivered'" class="flex flex-col gap-3">
                   <Button @click="acceptDelivery" :disabled="isSubmitting" class="w-full h-12 bg-teal-600 hover:bg-teal-700 text-white font-bold text-base shadow-md">
                     <CheckCircle2 class="w-5 h-5 mr-2" /> Zaakceptuj Dostawę
                   </Button>
                 </div>

                 <div class="border-t border-zinc-100 dark:border-zinc-800 pt-6">
                   <Label class="text-sm font-bold text-zinc-700 dark:text-zinc-300 mb-2 block">{{ orderInfo.status === 'delivered' ? 'Potrzebujesz poprawek?' : 'Twoja wiadomość' }}</Label>
                   <Textarea v-model="revisionMessage" :placeholder="orderInfo.status === 'delivered' ? 'Opisz dokładnie co sprzedawca powinien zmienić lub poprawić...' : 'Napisz coś więcej o swoim zamówieniu...'" class="min-h-[120px] mb-3 bg-zinc-50 dark:bg-zinc-950 border-zinc-200 dark:border-zinc-800" />
                   
                   <!-- Pliki do poprawki -->
                   <div class="mb-4">
                     <Label class="text-xs font-bold text-zinc-500 uppercase mb-2 block">Załącz pliki pomocnicze (opcjonalnie)</Label>
                     <div class="relative">
                       <input type="file" multiple @change="handleFileChange" class="absolute inset-0 w-full h-full opacity-0 cursor-pointer z-10" />
                       <div class="border-2 border-dashed border-zinc-200 dark:border-zinc-800 rounded-xl p-4 flex flex-col items-center justify-center text-zinc-500 hover:bg-zinc-50 dark:hover:bg-zinc-900/50 transition-colors">
                         <Upload class="w-5 h-5 mb-1 opacity-50" :class="orderInfo.status === 'delivered' ? 'text-amber-600' : 'text-teal-600'" />
                         <span class="text-xs">{{ selectedFiles.length > 0 ? `Wybrano ${selectedFiles.length} plik(ów)` : 'Kliknij, aby dodać pliki' }}</span>
                       </div>
                     </div>
                   </div>

                   <Button @click="requestRevision" :disabled="isSubmitting || !revisionMessage.trim()" variant="outline" class="w-full h-11" :class="orderInfo.status === 'delivered' ? 'border-amber-200 text-amber-700 hover:bg-amber-50 dark:border-amber-900/50 dark:text-amber-400 dark:hover:bg-amber-900/20' : 'border-teal-200 text-teal-700 hover:bg-teal-50 dark:border-teal-900/50 dark:text-teal-400 dark:hover:bg-teal-900/20'">
                     <template v-if="orderInfo.status === 'delivered'">
                        <RotateCcw class="w-4 h-4 mr-2" /> Poproś o Rewizję
                     </template>
                     <template v-else>
                        <Send class="w-4 h-4 mr-2" /> Wyślij wiadomość
                     </template>
                   </Button>
                 </div>
               </CardContent>
            </Card>

            <!-- Seller View: Deliver Work Card -->
            <Card v-else-if="isSeller && orderInfo.status !== 'completed'" class="border-indigo-200 dark:border-indigo-900/50 shadow-xl shadow-teal-900/5 bg-white dark:bg-zinc-900 relative overflow-hidden">
               <div class="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-indigo-400 to-indigo-600"></div>
               <CardHeader class="pb-3">
                 <CardTitle class="flex items-center gap-2 text-xl text-indigo-800 dark:text-indigo-400">
                   <Upload class="w-5 h-5" /> Dostarcz Pracę / Wyślij wiadomość
                 </CardTitle>
               </CardHeader>
               <CardContent class="flex flex-col gap-6">
                 <p v-if="orderInfo.status === 'delivered'" class="text-sm text-amber-600 dark:text-amber-400 font-medium">Oczekujesz na weryfikację dostarczonej pracy przez kupującego, ale możesz w międzyczasie przesłać nową wersję lub dodatkową wiadomość.</p>
                 <p v-else-if="orderInfo.status === 'revision'" class="text-sm text-zinc-600 dark:text-zinc-400 font-medium">Kupujący poprosił o poprawkę. Popraw pracę uwzględniając jego uwagi z osi czasu i prześlij zaktualizowaną wersję projektu.</p>
                 <p v-else class="text-sm text-zinc-600 dark:text-zinc-400 font-medium">Możesz wysłać kupującemu wiadomość, pliki robocze lub gotową pracę. Zostanie on powiadomiony o każdej nowej aktywności.</p>
                 
                 <div class="flex flex-col gap-4">
                   <div class="flex flex-col gap-2">
                     <Label class="text-sm font-bold text-zinc-700 dark:text-zinc-300">Twoja wiadomość</Label>
                     <Textarea v-model="deliveryMessage" placeholder="Opisz wysyłane pliki lub postępy w pracy..." class="min-h-[120px] bg-zinc-50 dark:bg-zinc-950 border-zinc-200 dark:border-zinc-800" />
                   </div>
                   
                   <!-- Prawdziwe dodawanie pliku -->
                   <div class="relative mb-2">
                     <input type="file" multiple @change="handleFileChange" class="absolute inset-0 w-full h-full opacity-0 cursor-pointer z-10" />
                     <div class="border-2 border-dashed border-zinc-200 dark:border-zinc-800 rounded-xl p-6 flex flex-col items-center justify-center text-zinc-500 hover:bg-zinc-50 dark:hover:bg-zinc-900/50 transition-colors">
                       <Upload class="w-8 h-8 mb-2 opacity-50 text-indigo-600" />
                       <span class="font-medium text-sm">{{ selectedFiles.length > 0 ? `Wybrano ${selectedFiles.length} plik(ów)` : 'Kliknij, aby załączyć pliki (Max 5GB)' }}</span>
                     </div>
                   </div>

                   <Button @click="deliverWork" :disabled="isSubmitting || !deliveryMessage.trim()" class="w-full h-12 bg-indigo-600 hover:bg-indigo-700 text-white font-bold text-base shadow-md mt-2">
                     <Send class="w-5 h-5 mr-2" /> {{ orderInfo.status === 'active' || orderInfo.status === 'paid' ? 'Wyślij aktualizację' : 'Dostarcz Pracę' }}
                   </Button>
                 </div>
               </CardContent>
            </Card>

            <Card v-if="orderInfo.status === 'completed'" class="border-zinc-200 dark:border-zinc-800 bg-zinc-50 dark:bg-zinc-900">
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
