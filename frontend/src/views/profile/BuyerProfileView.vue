<script setup>
import { ref, reactive, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useAuth } from "@/stores/auth";
import LandingHeader from "@/components/landing/LandingHeader.vue";
import Container from "@/components/ui/Container.vue";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Settings, UserCircle, Calendar, ShoppingBag, Clock, Package, ExternalLink, RefreshCw } from "lucide-vue-next";

const router = useRouter();
const { isLoggedIn, initAuth } = useAuth();
const loading = ref(true);

onMounted(async () => {
  await initAuth();
  if (!isLoggedIn.value) {
    router.push("/login");
    return;
  }
  fetchProfileInfo();
});

const ProfileInfo = reactive({
  firstName: "",
  lastName: "",
  JoinDate: "",
  TotalOrders: null,
  LastOrder: "",
});

const fetchProfileInfo = async () => {
  try {
    const response = await fetch("/api/Buyer/me");

    if (!response.ok) {
      const errorText = await response.text();
      console.error("Server Error:", response.status, errorText);
      return;
    }
    const result = await response.json();
    ProfileInfo.firstName = result.firstName;
    ProfileInfo.lastName = result.lastName;
    ProfileInfo.JoinDate = result.joinedDate?.slice(0, 10) ?? "Brak danych";
    ProfileInfo.TotalOrders = result.totalOrders ?? 0;
    ProfileInfo.LastOrder = result.lastOrderDate?.slice(0, 10) ?? "Brak zamówień";
  } catch (err) {
    console.error("Profile Error:", err);
  } finally {
    loading.value = false;
  }
};
const openOrderRevision = (id) => {
  router.push(`/order/${id}/revision`);
};
</script>

<template>
  <div class="min-h-svh bg-zinc-50 dark:bg-zinc-950 pb-20">
    <LandingHeader />
    
    <Container>
      <div class="mt-8 w-full flex flex-col gap-6">
        
        <!-- Header Row -->
        <div class="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
          <div>
            <h1 class="text-3xl font-extrabold text-zinc-900 dark:text-zinc-50">Profil Kupującego</h1>
            <p class="text-zinc-500 dark:text-zinc-400 mt-1">Podsumowanie Twojej aktywności w serwisie.</p>
          </div>
          <Button 
            variant="outline" 
            @click="router.push('/buyer/profile/settings')"
            class="border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 hover:bg-zinc-50 dark:hover:bg-zinc-800"
          >
            <Settings class="h-4 w-4 mr-2" /> Ustawienia Konta
          </Button>
        </div>

        <div v-if="loading" class="animate-pulse space-y-6">
           <div class="h-40 bg-zinc-200 dark:bg-zinc-800 rounded-2xl w-full"></div>
           <div class="h-64 bg-zinc-200 dark:bg-zinc-800 rounded-2xl w-full"></div>
        </div>

        <template v-else>
          <!-- Main Profile Card -->
          <Card class="w-full shadow-lg shadow-teal-900/5 border-zinc-200 dark:border-zinc-800 overflow-hidden relative">
            <div class="absolute right-0 top-0 w-32 h-32 bg-teal-500/10 blur-3xl rounded-full -mr-16 -mt-16 pointer-events-none"></div>
            <CardContent class="p-8 md:p-10 flex flex-col md:flex-row gap-8 items-center md:items-start text-center md:text-left">
              
              <div class="h-24 w-24 rounded-full bg-teal-100 dark:bg-teal-900/40 flex items-center justify-center shrink-0 border-4 border-white dark:border-zinc-950 shadow-sm">
                <UserCircle class="h-12 w-12 text-teal-600 dark:text-teal-400" />
              </div>

              <div class="flex-1 flex flex-col gap-4">
                <h2 class="text-3xl font-black text-zinc-900 dark:text-zinc-50">
                  Cześć, {{ ProfileInfo.firstName }} {{ ProfileInfo.lastName }}!
                </h2>
                
                <div class="grid grid-cols-1 sm:grid-cols-3 gap-4 mt-2">
                  <div class="flex items-center gap-3 bg-zinc-50 dark:bg-zinc-900/50 p-3 rounded-xl border border-zinc-100 dark:border-zinc-800/50">
                    <Calendar class="h-5 w-5 text-teal-600 dark:text-teal-400" />
                    <div class="flex flex-col text-left">
                      <span class="text-xs font-bold uppercase tracking-wider text-zinc-500">Dołączenie</span>
                      <span class="font-semibold text-zinc-900 dark:text-zinc-100">{{ ProfileInfo.JoinDate }}</span>
                    </div>
                  </div>

                  <div class="flex items-center gap-3 bg-zinc-50 dark:bg-zinc-900/50 p-3 rounded-xl border border-zinc-100 dark:border-zinc-800/50">
                    <ShoppingBag class="h-5 w-5 text-teal-600 dark:text-teal-400" />
                    <div class="flex flex-col text-left">
                      <span class="text-xs font-bold uppercase tracking-wider text-zinc-500">Zamówień</span>
                      <span class="font-semibold text-zinc-900 dark:text-zinc-100">{{ ProfileInfo.TotalOrders }}</span>
                    </div>
                  </div>

                  <div class="flex items-center gap-3 bg-zinc-50 dark:bg-zinc-900/50 p-3 rounded-xl border border-zinc-100 dark:border-zinc-800/50">
                    <Clock class="h-5 w-5 text-teal-600 dark:text-teal-400" />
                    <div class="flex flex-col text-left">
                      <span class="text-xs font-bold uppercase tracking-wider text-zinc-500">Ostatnie Zam.</span>
                      <span class="font-semibold text-zinc-900 dark:text-zinc-100">{{ ProfileInfo.LastOrder }}</span>
                    </div>
                  </div>
                </div>
              </div>

            </CardContent>
          </Card>
          
          <!-- Orders Summary Card -->
          <Card class="w-full shadow-lg shadow-teal-900/5 border-zinc-200 dark:border-zinc-800">
            <CardHeader class="pb-2 border-b border-zinc-100 dark:border-zinc-800/50">
              <CardTitle class="text-xl">Twoje Zamówienia</CardTitle>
            </CardHeader>
            <CardContent class="p-0">
              <div class="p-6 md:p-8 flex flex-col gap-8">
                
                <div>
                  <h3 class="text-lg font-bold mb-3 text-zinc-900 dark:text-zinc-100 flex items-center gap-2">
                    <span class="w-2 h-2 rounded-full bg-teal-500"></span>
                    W trakcie (In Progress)
                  </h3>
                  
                  <!-- Placeholder Order -->
                  <div 
                    class="group relative bg-white dark:bg-zinc-900 rounded-xl p-5 border border-zinc-200 dark:border-zinc-800 shadow-sm hover:shadow-md hover:border-teal-200 dark:hover:border-teal-900/50 transition-all cursor-pointer flex flex-col md:flex-row md:items-center justify-between gap-4"
                    @click="openOrderRevision('ORD-001')"
                  >
                    <div class="flex items-center gap-4">
                      <div class="h-12 w-12 shrink-0 rounded-lg bg-teal-50 dark:bg-teal-900/30 flex items-center justify-center text-teal-600 dark:text-teal-400">
                        <RefreshCw class="h-6 w-6 font-bold" />
                      </div>
                      <div class="flex flex-col">
                        <span class="text-sm font-bold text-zinc-400">Zamówienie #ORD-001</span>
                        <h4 class="font-bold text-zinc-900 dark:text-zinc-100 text-lg">Nowoczesna strona internetowa WordPress</h4>
                        <span class="text-sm text-zinc-500 dark:text-zinc-400 font-medium">Od: WebDevPro</span>
                      </div>
                    </div>
                    
                    <div class="flex items-center gap-4 md:border-l border-zinc-100 dark:border-zinc-800 md:pl-6 min-w-40">
                      <div class="flex flex-col">
                        <span class="text-sm text-zinc-500">Status</span>
                        <span class="font-bold text-blue-600 dark:text-blue-400 flex items-center gap-1.5">
                          Oczekuje na recenzję
                        </span>
                      </div>
                      <Button variant="ghost" size="icon" class="ml-auto text-zinc-400 group-hover:text-teal-600 transition-colors">
                        <ExternalLink class="h-5 w-5" />
                      </Button>
                    </div>
                  </div>

                </div>
                
                <div>
                  <h3 class="text-lg font-bold mb-3 text-zinc-900 dark:text-zinc-100 flex items-center gap-2">
                    <span class="w-2 h-2 rounded-full bg-zinc-300 dark:bg-zinc-700"></span>
                    Zakończone (Finished)
                  </h3>
                  <div class="bg-zinc-50 dark:bg-zinc-900/50 rounded-xl p-6 text-center border border-zinc-100 dark:border-zinc-800 border-dashed">
                    <span class="text-zinc-500 font-medium select-none">Brak zakończonych zamówień.</span>
                  </div>
                </div>

              </div>
            </CardContent>
          </Card>
        </template>

      </div>
    </Container>
  </div>
</template>
