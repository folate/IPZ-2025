<script setup>
import { ref } from "vue";
import { useRouter, useRoute } from "vue-router";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

const router = useRouter();
const route = useRoute();

const email = ref("");
const password = ref("");
const serverNotification = ref("");
const isLoading = ref(false);

const handleLogin = async () => {
  serverNotification.value = "";
  
  if (!email.value || !password.value) {
    serverNotification.value = "Podaj login i hasło.";
    return;
  }

  isLoading.value = true;

  try {
    const response = await fetch("/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        Login: email.value,
        Password: password.value,
        doNotLogOut: false,
      }),
      credentials: "include",
    });

    if (!response.ok) {
      if (response.status === 400 || response.status === 401) {
        serverNotification.value = "Nieprawidłowy login lub hasło.";
      } else {
        serverNotification.value = "Wystąpił błąd. Spróbuj ponownie.";
      }
      return;
    }

    // Success
    const redirectPath = route.query.redirect || "/";
    router.push(redirectPath);
  } catch (err) {
    console.error("Network Error:", err);
    serverNotification.value = "Błąd połączenia z serwerem.";
  } finally {
    isLoading.value = false;
  }
};
</script>

<template>
  <div class="flex min-h-svh w-full items-center justify-center p-6 md:p-10 bg-zinc-50 dark:bg-zinc-950">
    <div class="w-full max-w-4xl">
      <Card class="overflow-hidden p-0 border-zinc-200 dark:border-zinc-800 shadow-xl shadow-teal-900/5">
        <CardContent class="grid p-0 md:grid-cols-2">
          
          <!-- Left side: Form -->
          <div class="p-6 md:p-8 flex flex-col justify-center">
            <div class="flex flex-col items-center gap-2 text-center mb-8">
              <h1 class="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">Logowanie</h1>
              <p class="text-zinc-500 dark:text-zinc-400">
                Wprowadź swój login i hasło, aby uzyskać dostęp
              </p>
            </div>
            
            <form @submit.prevent="handleLogin" class="flex flex-col gap-6">
              <!-- Error Message -->
              <div v-if="serverNotification" class="p-3 text-sm bg-red-50 dark:bg-red-900/20 text-red-600 dark:text-red-400 rounded-lg border border-red-100 dark:border-red-900/30 font-medium text-center">
                {{ serverNotification }}
              </div>

              <div class="grid gap-2">
                <Label for="email" class="text-zinc-700 dark:text-zinc-300">Login</Label>
                <Input
                  id="email"
                  type="text"
                  placeholder="Twój login"
                  v-model="email"
                  required
                  class="h-11 rounded-xl focus-visible:ring-teal-500 border-zinc-200 dark:border-zinc-800 dark:bg-zinc-900"
                />
              </div>
              <div class="grid gap-2">
                <div class="flex items-center">
                  <Label for="password" class="text-zinc-700 dark:text-zinc-300">Hasło</Label>
                  <a
                    href="#"
                    class="ml-auto inline-block text-sm text-teal-600 dark:text-teal-400 hover:text-teal-700 hover:underline"
                  >
                    Zapomniałeś hasła?
                  </a>
                </div>
                <Input 
                  id="password" 
                  type="password" 
                  v-model="password" 
                  required 
                  class="h-11 rounded-xl focus-visible:ring-teal-500 border-zinc-200 dark:border-zinc-800 dark:bg-zinc-900" 
                />
              </div>
              
              <Button type="submit" class="w-full h-11 rounded-xl bg-teal-600 hover:bg-teal-700 text-white shadow-md font-semibold text-base transition-colors mt-2" :disabled="isLoading">
                <span v-if="isLoading" class="flex items-center gap-2">
                  <div class="w-4 h-4 rounded-full border-2 border-white border-t-transparent animate-spin"></div>
                  Logowanie...
                </span>
                <span v-else>Zaloguj się</span>
              </Button>

              <div class="mt-4 text-center text-sm text-zinc-500 dark:text-zinc-400">
                Nie masz jeszcze konta?
                <router-link :to="route.query.redirect ? `/register?redirect=${route.query.redirect}` : '/register'" class="font-medium inline-block text-teal-600 dark:text-teal-400 hover:text-teal-700 hover:underline ml-1">
                  Zarejestruj się
                </router-link>
              </div>
            </form>
          </div>

          <!-- Right side: Image -->
          <div class="bg-zinc-100 dark:bg-zinc-900 relative hidden md:block">
            <img
              src="https://images.unsplash.com/photo-1497366216548-37526070297c?q=80&w=1500&auto=format&fit=crop"
              alt="Workspace"
              class="absolute inset-0 h-full w-full object-cover dark:brightness-[0.7] dark:grayscale-[0.3]"
            />
            <div class="absolute inset-0 bg-teal-900/10 mix-blend-multiply dark:bg-teal-900/30"></div>
          </div>

        </CardContent>
      </Card>
      
      <div class="mt-6 text-center text-sm text-zinc-500 dark:text-zinc-400">
        Klikając kontynuuj, zgadzasz się na nasze <a href="#" class="underline hover:text-teal-600">Warunki świadczenia usług</a>
        i <a href="#" class="underline hover:text-teal-600">Politykę Prywatności</a>.
      </div>
    </div>
  </div>
</template>
