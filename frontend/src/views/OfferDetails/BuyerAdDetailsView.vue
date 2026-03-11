<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import LandingHeader from "@/components/landing/LandingHeader.vue";
import Container from "@/components/ui/Container.vue";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Loader2, Calendar, DollarSign, Tag, User } from "lucide-vue-next";

const route = useRoute();
const loading = ref(false);
const error = ref("");
const ad = ref(null);

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
  } catch {
    error.value = "Błąd sieci przy pobieraniu zlecenia.";
  } finally {
    loading.value = false;
  }
}

onMounted(load);
</script>

<template>
  <div class="min-h-svh bg-zinc-50 dark:bg-zinc-950 pb-20">
    <LandingHeader />

    <Container>
      <div class="mt-8 w-full flex flex-col gap-6">
        
        <div v-if="loading" class="flex justify-center py-20">
          <Loader2 class="h-10 w-10 text-teal-600 animate-spin" />
        </div>

        <div v-else-if="error" class="bg-red-50 dark:bg-red-900/10 text-red-600 dark:text-red-400 p-6 rounded-2xl font-medium border border-red-200 dark:border-red-900/30">
          {{ error }}
        </div>

        <Card v-else-if="ad" class="border-zinc-200 dark:border-zinc-800 shadow-xl shadow-teal-900/5 overflow-hidden">
          <div class="h-2 w-full bg-gradient-to-r from-teal-500 to-teal-700"></div>
          <CardHeader class="pb-4">
            <div class="flex justify-between items-start gap-4">
               <div>
                 <CardTitle class="text-3xl font-extrabold text-zinc-900 dark:text-zinc-50 leading-tight">
                   {{ ad.title }}
                 </CardTitle>
                 <CardDescription class="text-zinc-500 dark:text-zinc-400 text-lg mt-2 flex items-center gap-2">
                   <Tag class="h-4 w-4" /> {{ ad.category }}
                 </CardDescription>
               </div>
               <div class="flex flex-col items-end text-right bg-zinc-100 dark:bg-zinc-900 p-3 rounded-xl border border-zinc-200 dark:border-zinc-800 shrink-0 min-w-32">
                 <span class="text-xs font-bold uppercase tracking-wider text-zinc-500 dark:text-zinc-400 flex items-center gap-1"><DollarSign class="h-3 w-3" /> Budżet</span>
                 <span class="text-xl font-black text-teal-600 dark:text-teal-400 mt-1">{{ ad.budget }} zł</span>
               </div>
            </div>
          </CardHeader>
          
          <CardContent class="flex flex-col gap-8 pt-2">
            
            <div class="prose dark:prose-invert max-w-none text-zinc-700 dark:text-zinc-300">
              <p class="whitespace-pre-line text-lg leading-relaxed font-medium">
                {{ ad.description }}
              </p>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 pt-6 border-t border-zinc-100 dark:border-zinc-800">
              
              <!-- Deadline -->
              <div class="flex items-center gap-4 bg-zinc-50 dark:bg-zinc-900/50 p-4 rounded-xl border border-zinc-100 dark:border-zinc-800">
                <div class="h-12 w-12 bg-white dark:bg-zinc-950 rounded-full flex items-center justify-center border border-zinc-200 dark:border-zinc-800 shadow-sm">
                  <Calendar class="h-6 w-6 text-teal-600 dark:text-teal-400" />
                </div>
                <div>
                  <p class="text-sm font-bold text-zinc-500 dark:text-zinc-400 uppercase tracking-widest">Termin Wykonania</p>
                  <p class="text-lg font-bold text-zinc-900 dark:text-zinc-50">{{ String(ad.deadline).slice(0, 10) }}</p>
                </div>
              </div>

              <!-- Client -->
              <div v-if="ad.buyerName" class="flex items-center gap-4 bg-zinc-50 dark:bg-zinc-900/50 p-4 rounded-xl border border-zinc-100 dark:border-zinc-800">
                <div class="h-12 w-12 bg-white dark:bg-zinc-950 rounded-full flex items-center justify-center border border-zinc-200 dark:border-zinc-800 shadow-sm">
                  <User class="h-6 w-6 text-zinc-600 dark:text-zinc-400" />
                </div>
                <div>
                  <p class="text-sm font-bold text-zinc-500 dark:text-zinc-400 uppercase tracking-widest">Zleceniodawca</p>
                  <p class="text-lg font-bold text-zinc-900 dark:text-zinc-50">{{ ad.buyerName }}</p>
                </div>
              </div>

            </div>

          </CardContent>
        </Card>

      </div>
    </Container>
  </div>
</template>
