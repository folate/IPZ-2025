<script setup>
import { ref, onMounted } from "vue";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Loader2 } from "lucide-vue-next";

const details = ref({
  ShippingAddress: "",
  BillingAdress: "",
  PreferredPaymentMethod: "PayPal",
});

const previousDetails = ref({});
const isEditing = ref(false);
const isLoading = ref(false);
const isSaving = ref(false);

function changeSettings() {
  isEditing.value = true;
}

async function savePreferences() {
  isSaving.value = true;
  const payload = {
    shippingAddress: details.value.ShippingAddress.trim() || previousDetails.value.ShippingAddress,
    billingAddress: details.value.BillingAdress.trim() || previousDetails.value.BillingAdress,
    preferredPaymentMethod: details.value.PreferredPaymentMethod,
  };
  
  try {
    const response = await fetch(`/api/Buyer/me`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
    if (!response.ok) {
      const errorText = await response.text();
      console.error("Server Error:", response.status, errorText);
      return;
    }
    isEditing.value = false;
    await fetchProfileInfo();
  } catch (err) {
    console.error("Profile Error:", err);
  } finally {
    isSaving.value = false;
  }
}

async function fetchProfileInfo() {
  isLoading.value = true;
  try {
    const response = await fetch("/api/Buyer/me");

    if (!response.ok) {
      const errorText = await response.text();
      console.error("Server Error:", response.status, errorText);
      return;
    }
    const result = await response.json();
    const data = {
      ShippingAddress: result.shippingAddress || "",
      BillingAdress: result.billingAddress || "",
      PreferredPaymentMethod: result.preferredPaymentMethod || "PayPal",
    };
    details.value = { ...data };
    previousDetails.value = { ...data };
    console.log("Success fetching profile info");
  } catch (err) {
    console.error("Profile Error:", err);
  } finally {
    isLoading.value = false;
  }
}

function CancelSave() {
  details.value = { ...previousDetails.value };
  isEditing.value = false;
}

onMounted(fetchProfileInfo);
</script>

<template>
  <div class="flex flex-col gap-6">
    <div class="flex flex-col gap-1">
      <h2 class="text-xl font-bold text-zinc-900 dark:text-zinc-50">Domyślne ustawienia zakupów</h2>
      <p class="text-zinc-500 dark:text-zinc-400 text-sm">Zdefiniuj domyślne dane, które będą automatycznie wypełniane przy zamówieniach.</p>
    </div>

    <div v-if="isLoading" class="flex justify-center py-8">
      <Loader2 class="h-8 w-8 text-teal-600 animate-spin" />
    </div>

    <div v-else class="flex flex-col gap-6 max-w-xl">
      
      <!-- Shipping Address -->
      <div class="flex flex-col gap-2">
        <Label class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Adres Dostawy</Label>
        <div v-if="isEditing">
          <Input 
            v-model="details.ShippingAddress" 
            :placeholder="previousDetails.ShippingAddress || 'Wprowadź adres...'"
            class="h-11 rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus-visible:ring-teal-500" 
          />
          <p v-if="!details.ShippingAddress.trim()" class="text-xs text-amber-600 dark:text-amber-500 mt-1">
            Pole jest puste. Przywrócono do: {{ previousDetails.ShippingAddress }}
          </p>
        </div>
        <div v-else class="h-11 px-3 py-2 flex items-center bg-zinc-100/50 dark:bg-zinc-800/30 border border-transparent rounded-xl text-zinc-900 dark:text-zinc-100">
           {{ details.ShippingAddress || 'Brak danych' }}
        </div>
      </div>

      <!-- Billing Address -->
      <div class="flex flex-col gap-2">
        <Label class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Adres Rozliczeniowy</Label>
        <div v-if="isEditing">
          <Input 
            v-model="details.BillingAdress" 
            :placeholder="previousDetails.BillingAdress || 'Wprowadź adres...'"
            class="h-11 rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus-visible:ring-teal-500" 
          />
          <p v-if="!details.BillingAdress.trim()" class="text-xs text-amber-600 dark:text-amber-500 mt-1">
            Pole jest puste. Przywrócono do: {{ previousDetails.BillingAdress }}
          </p>
        </div>
        <div v-else class="h-11 px-3 py-2 flex items-center bg-zinc-100/50 dark:bg-zinc-800/30 border border-transparent rounded-xl text-zinc-900 dark:text-zinc-100">
           {{ details.BillingAdress || 'Brak danych' }}
        </div>
      </div>

      <!-- Preferred Payment Method -->
      <div class="flex flex-col gap-2">
        <Label class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Preferowana Metoda Płatności</Label>
        <div v-if="isEditing">
          <Select v-model="details.PreferredPaymentMethod">
            <SelectTrigger class="h-11 rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus:ring-teal-500 w-full sm:w-64">
              <SelectValue placeholder="Wybierz metodę płatności" />
            </SelectTrigger>
            <SelectContent class="bg-white dark:bg-zinc-950 border-zinc-200 dark:border-zinc-800 rounded-xl">
              <SelectGroup>
                <SelectItem value="PayPal" class="focus:bg-zinc-100 dark:focus:bg-zinc-900 focus:text-teal-700 dark:focus:text-teal-400 cursor-pointer">PayPal</SelectItem>
                <SelectItem value="Debit" class="focus:bg-zinc-100 dark:focus:bg-zinc-900 focus:text-teal-700 dark:focus:text-teal-400 cursor-pointer">Karta Debetowa</SelectItem>
                <SelectItem value="Blik" class="focus:bg-zinc-100 dark:focus:bg-zinc-900 focus:text-teal-700 dark:focus:text-teal-400 cursor-pointer">Blik</SelectItem>
              </SelectGroup>
            </SelectContent>
          </Select>
        </div>
        <div v-else class="h-11 px-3 py-2 flex items-center bg-zinc-100/50 dark:bg-zinc-800/30 border border-transparent rounded-xl text-zinc-900 dark:text-zinc-100">
           {{ details.PreferredPaymentMethod }}
        </div>
      </div>

    </div>

    <!-- Actions -->
    <div v-if="!isLoading" class="pt-4 border-t border-zinc-200 dark:border-zinc-800 flex items-center gap-3">
      <template v-if="isEditing">
        <Button @click="savePreferences" :disabled="isSaving" class="bg-teal-600 hover:bg-teal-700 text-white font-medium min-w-24">
          <Loader2 v-if="isSaving" class="w-4 h-4 mr-2 animate-spin" />
          Zapisz
        </Button>
        <Button variant="outline" @click="CancelSave" :disabled="isSaving" class="border-zinc-200 dark:border-zinc-800 hover:bg-zinc-100 dark:hover:bg-zinc-800">
          Anuluj
        </Button>
      </template>
      <template v-else>
        <Button @click="changeSettings" class="bg-zinc-900 dark:bg-zinc-50 text-white dark:text-zinc-900 hover:bg-zinc-800 dark:hover:bg-zinc-200 font-medium">
          Edytuj Dane
        </Button>
      </template>
    </div>
  </div>
</template>
