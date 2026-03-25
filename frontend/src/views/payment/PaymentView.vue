<script setup>
import { ref, onMounted } from "vue"
import { useRoute, useRouter } from "vue-router"
import { useAlert } from "@/stores/alert"
import { ErrorMessage, Field, Form } from "vee-validate"
import * as yup from "yup"

import { useAuth } from "@/stores/auth"

import Container from "@/components/ui/Container.vue"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Button } from "@/components/ui/button"
import { CreditCard, Loader2, CheckCircle2, ShieldCheck, Lock } from "lucide-vue-next"

const route = useRoute()
const router = useRouter()
const { isLoggedIn, initAuth } = useAuth()
const { showAlert } = useAlert()

const isProcessing = ref(false)
const isSuccess = ref(false)
const pageLoading = ref(true)

const preferredPaymentMethod = ref("")

const fetchBuyerInfo = async () => {
  try {
    const res = await fetch("/api/Buyer/me", { credentials: "include" })
    if (res.ok) {
      const buyerData = await res.json()
      if (buyerData.preferredPaymentMethod) {
         preferredPaymentMethod.value = buyerData.preferredPaymentMethod;
      }
    }
  } catch(e) { console.error("Error fetching buyer info:", e) }
}

const orderDetails = ref({
  gigId: route.query.gigId,
  sellerId: route.query.sellerId,
  price: route.query.price || '0',
  tierName: route.query.tierName || 'Nieznany pakiet',
  title: route.query.title || 'Usługa'
})

onMounted(async () => {
  await initAuth()
  
  if (!isLoggedIn.value) {
    router.push({
      path: '/login',
      query: { redirect: route.fullPath } // redirect back after login if handled
    })
    return
  }

  if (!orderDetails.value.gigId) {
    router.push('/')
    return
  }
  
  await fetchBuyerInfo();
  
  pageLoading.value = false
})

const schema = yup.object({
  additionalInstructions: yup.string().optional()
})

async function onSubmit(values) {
  isProcessing.value = true
  
  try {
    const res = await fetch(`/api/Order/create`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        quantity: 1,
        additionalInstructions: values.additionalInstructions || "",
        aproxDeliveryTime: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
        gigId: Number(orderDetails.value.gigId)
      }),
    });
    
    if (!res.ok) {
      throw new Error(`Błąd zakupu (${res.status})`);
    }
    
    const data = await res.json();
    
    if (data && data.url) {
      // PayU returned a Redirect URL
      window.location.href = data.url;
    } else {
      isProcessing.value = false
      isSuccess.value = true
      
      setTimeout(() => {
        router.push("/thanks") 
      }, 2000)
    }
    
  } catch (err) {
    console.error(err)
    showAlert("Błąd płatności", "Wystąpił błąd podczas przetwarzania płatności: " + err.message, "destructive")
    isProcessing.value = false
  }
}
</script>

<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20">

    <Container>
      
      <!-- Loading State for Auth Check -->
      <div v-if="pageLoading" class="mt-20 flex flex-col items-center justify-center">
        <Loader2 class="h-10 w-10 animate-spin text-teal-600 mb-4" />
        <p class="text-zinc-500 font-medium">Sprawdzanie autoryzacji...</p>
      </div>

      <!-- Success Screen -->
      <div v-else-if="isSuccess" class="mt-12 flex justify-center w-full">
        <Card class="border-zinc-200 dark:border-zinc-800 shadow-xl shadow-teal-900/5 max-w-2xl w-full py-16">
          <CardContent class="flex flex-col items-center justify-center p-6 text-center">
            <div class="h-24 w-24 rounded-full bg-teal-100 dark:bg-teal-900/30 flex items-center justify-center mb-8">
              <CheckCircle2 class="h-12 w-12 text-teal-600 dark:text-teal-400" />
            </div>
            <h2 class="text-3xl font-black text-zinc-900 dark:text-white mb-3">Płatność pomyślna!</h2>
            <p class="text-zinc-500 dark:text-zinc-400 max-w-md text-lg">Twoje zamówienie zostało przekazane do realizacji. Dziękujemy za zaufanie.</p>
            <p class="text-sm text-zinc-400 font-medium mt-10 flex items-center gap-2">
              <Loader2 class="h-4 w-4 animate-spin" /> Przekierowywanie do profilu...
            </p>
          </CardContent>
        </Card>
      </div>

      <!-- Checkout View -->
      <div v-else class="mt-8">
        
        <div class="mb-8">
          <h1 class="text-3xl md:text-4xl font-extrabold text-zinc-900 dark:text-zinc-50">Kasa</h1>
          <p class="text-zinc-500 dark:text-zinc-400 mt-2 text-lg">Wprowadź dane płatności, aby sfinalizować zamówienie.</p>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-12 gap-8 lg:gap-12">
          
          <!-- Left Column: Payment Form -->
          <div class="lg:col-span-8">
            <Card class="border-zinc-200 dark:border-zinc-800 shadow-xl shadow-teal-900/5 overflow-hidden">
              <div class="bg-zinc-100/50 dark:bg-zinc-900/50 px-6 py-4 border-b border-zinc-200 dark:border-zinc-800 flex flex-wrap justify-between items-center gap-3">
                <div class="flex items-center gap-3">
                  <h2 class="text-xl font-bold text-zinc-900 dark:text-zinc-50">Informacje do zamówienia</h2>
                </div>
              </div>
              
              <CardContent class="p-6 md:p-8">
                <!-- Using ref on Form to allow external submission -->
                <Form id="paymentForm"
                  :validation-schema="schema"
                  :initial-values="{
                    additionalInstructions: ''
                  }"
                  @submit="onSubmit"
                >
                  <div class="flex flex-col gap-6">
                    
                    <!-- Additional Instructions -->
                    <div class="flex flex-col gap-2">
                      <Label for="additionalInstructions" class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Dodatkowe instrukcje dla sprzedawcy (opcjonalne)</Label>
                      <Field name="additionalInstructions" v-slot="{ field }">
                        <Textarea id="additionalInstructions" v-bind="field" placeholder="Masz jakieś szczególne wymagania co do tego zamówienia?" class="min-h-[100px] rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus-visible:ring-teal-500 resize-y" />
                      </Field>
                      <div class="h-4"><ErrorMessage name="additionalInstructions" class="text-xs text-red-500 font-medium block" /></div>
                    </div>

                  </div>
                </Form>
              </CardContent>
            </Card>
          </div>

          <!-- Right Column: Order Summary (Sticky) -->
          <div class="lg:col-span-4 flex flex-col gap-6">
            <div class="sticky top-24">
              <Card class="border-zinc-200 dark:border-zinc-800 shadow-xl shadow-teal-900/5 overflow-hidden">
                <CardHeader class="pb-4 border-b border-zinc-100 dark:border-zinc-800/50 bg-zinc-50/50 dark:bg-zinc-900/30">
                  <CardTitle class="text-lg">Podsumowanie zamówienia</CardTitle>
                </CardHeader>
                <CardContent class="p-6">
                  
                  <div class="flex flex-col gap-4 mb-6">
                    <div class="flex flex-col">
                      <span class="text-sm font-bold text-zinc-400 uppercase tracking-wider mb-1">Usługa</span>
                      <span class="font-bold text-zinc-900 dark:text-zinc-100 text-lg leading-tight">{{ orderDetails.title }}</span>
                    </div>
                    
                    <div class="flex flex-col bg-zinc-50 dark:bg-zinc-900/50 p-4 rounded-xl border border-zinc-100 dark:border-zinc-800">
                      <span class="text-xs font-bold text-teal-600 dark:text-teal-400 uppercase tracking-wider mb-1">Wybrany Pakiet</span>
                      <span class="font-semibold text-zinc-800 dark:text-zinc-200">{{ orderDetails.tierName }}</span>
                    </div>
                  </div>

                  <div class="space-y-3 mb-6">
                    <div class="flex justify-between items-center text-zinc-600 dark:text-zinc-400">
                      <span>Kwota netto</span>
                      <span class="font-medium">{{ (Number(orderDetails.price) * 0.77).toFixed(2) }} zł</span>
                    </div>
                    <div class="flex justify-between items-center text-zinc-600 dark:text-zinc-400">
                      <span>VAT (23%)</span>
                      <span class="font-medium">{{ (Number(orderDetails.price) * 0.23).toFixed(2) }} zł</span>
                    </div>
                  </div>

                  <div class="flex justify-between items-center py-4 border-t border-b border-zinc-100 dark:border-zinc-800 mb-6">
                    <span class="font-bold text-zinc-900 dark:text-zinc-100 text-lg">Do zapłaty PLN</span>
                    <span class="text-3xl font-black text-teal-600 dark:text-teal-400">{{ orderDetails.price }} zł</span>
                  </div>

                  <Button 
                    form="paymentForm" 
                    type="submit" 
                    :disabled="isProcessing" 
                    class="w-full h-14 rounded-xl bg-teal-600 hover:bg-teal-700 text-white shadow-lg shadow-teal-900/20 font-bold text-lg transition-all flex items-center justify-center gap-2 group"
                  >
                    <Loader2 v-if="isProcessing" class="h-6 w-6 animate-spin" />
                    <template v-else>
                      Zapłać i Zamów
                    </template>
                  </Button>

                  <div class="mt-6 flex items-start gap-3 bg-teal-50 dark:bg-teal-900/20 p-4 rounded-xl text-teal-800 dark:text-teal-300 border border-teal-100 dark:border-teal-900/50">
                    <ShieldCheck class="h-5 w-5 shrink-0 mt-0.5" />
                    <p class="text-xs font-medium leading-relaxed">
                      Twoja płatność jest chroniona i szyfrowana algorytmem AES-256. Nigdy nie udostępniamy Twoich danych organizacjom trzecim.
                    </p>
                  </div>

                </CardContent>
              </Card>
            </div>
          </div>

        </div>
      </div>
      
    </Container>
  </div>
</template>
