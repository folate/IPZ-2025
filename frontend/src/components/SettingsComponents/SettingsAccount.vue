<script setup>
import { ref, onMounted } from "vue";
import { useAuth } from "@/stores/auth";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

const { state, hasRole } = useAuth();
const loading = ref(false);
const error = ref("");
const success = ref("");

const form = ref({
  firstName: "",
  lastName: "",
  email: "",
  login: state.user?.login || "",
  isFreelancer: state.user?.roles?.includes("Seller") || state.user?.roles?.includes("Freelancer") || false
});

const passwordForm = ref({
  login: state.user?.login || "",
  newPassword: ""
});
const passwordLoading = ref(false);
const passwordError = ref("");
const passwordSuccess = ref("");
const showPasswordDialog = ref(false);

onMounted(async () => {
    try {
        const endpoint = state.user?.roles?.includes("Buyer") ? "/api/Buyer/me" : "/api/Seller/me";
        const res = await fetch(endpoint, { credentials: "include" });
        if (res.ok) {
            const data = await res.json();
            form.value.firstName = data.firstName;
            form.value.lastName = data.lastName;
            form.value.email = data.email;
        }
    } catch(err) {
        console.error("Failed to load user info", err);
    }
});

const submitForm = async () => {
  loading.value = true;
  error.value = "";
  success.value = "";
  try {
    const res = await fetch("/api/Auth/modify", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(form.value),
    });

    if (res.ok) {
      success.value = "Dane konta zostały zaktualizowane pomyślnie.";
    } else {
      const msg = await res.text();
      error.value = msg || "Wystąpił błąd podczas aktualizacji.";
    }
  } catch (err) {
    error.value = err.message || "Błąd sieci";
  } finally {
    loading.value = false;
  }
};

const changePassword = async () => {
  passwordLoading.value = true;
  passwordError.value = "";
  passwordSuccess.value = "";
  try {
    const res = await fetch("/api/Auth/change-password", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(passwordForm.value),
    });

    if (res.ok) {
      passwordSuccess.value = "Hasło zostało zmienione pomyślnie.";
      passwordForm.value.newPassword = "";
      setTimeout(() => {
        showPasswordDialog.value = false;
        passwordSuccess.value = "";
      }, 2000);
    } else {
      const msg = await res.text();
      passwordError.value = msg || "Wystąpił błąd podczas zmiany hasła.";
    }
  } catch (err) {
    passwordError.value = err.message || "Błąd sieci";
  } finally {
    passwordLoading.value = false;
  }
};
</script>

<template>
  <Card class="border-zinc-200 dark:border-zinc-800 shadow-sm mb-6">
    <CardHeader class="border-b border-zinc-100 dark:border-zinc-800/50 pb-4">
      <CardTitle class="text-xl">Zmień dane konta lub hasło</CardTitle>
    </CardHeader>
    <CardContent class="p-6 md:p-8 flex flex-col gap-6">
      
      <div v-if="error" class="p-4 bg-red-50 text-red-600 rounded-xl font-medium">
        {{ error }}
      </div>
      <div v-if="success" class="p-4 bg-teal-50 text-teal-700 rounded-xl font-medium">
        {{ success }}
      </div>

      <form @submit.prevent="submitForm" class="flex flex-col gap-5">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="space-y-2">
                <Label for="firstName">Imię</Label>
                <Input id="firstName" v-model="form.firstName" required />
            </div>
            
            <div class="space-y-2">
                <Label for="lastName">Nazwisko</Label>
                <Input id="lastName" v-model="form.lastName" required />
            </div>
            
            <div class="space-y-2">
                <Label for="email">E-mail</Label>
                <Input id="email" type="email" v-model="form.email" required />
            </div>

            <div class="space-y-2">
                <Label for="login">Login (Nazwa użytkownika)</Label>
                <Input id="login" disabled v-model="form.login" required class="bg-zinc-100 dark:bg-zinc-900" />
            </div>
        </div>

        <div class="flex flex-col md:flex-row justify-between items-start md:items-center pt-4 border-t border-zinc-100 dark:border-zinc-800 gap-4">
          <div class="space-y-1">
            <h4 class="font-medium">Hasło konta</h4>
            <p class="text-sm text-zinc-500">Zalecamy regularną zmianę hasła dla bezpieczeństwa.</p>
          </div>
          
          <Dialog v-model:open="showPasswordDialog">
            <DialogTrigger as-child>
              <Button type="button" variant="outline" class="border-zinc-200">Zmień hasło</Button>
            </DialogTrigger>
            <DialogContent class="sm:max-w-[425px]">
              <DialogHeader>
                <DialogTitle>Zmień hasło</DialogTitle>
                <DialogDescription>
                  Wpisz nowe hasło poniżej. Musi mieć co najmniej 8 znaków.
                </DialogDescription>
              </DialogHeader>
              <div class="grid gap-4 py-4">
                <div v-if="passwordError" class="p-3 bg-red-50 text-red-600 rounded-lg text-sm">
                  {{ passwordError }}
                </div>
                <div v-if="passwordSuccess" class="p-3 bg-teal-50 text-teal-700 rounded-lg text-sm">
                  {{ passwordSuccess }}
                </div>
                <div class="grid gap-2">
                  <Label for="new-password">Nowe hasło</Label>
                  <Input 
                    id="new-password" 
                    type="password" 
                    v-model="passwordForm.newPassword" 
                    placeholder="Min. 8 znaków"
                  />
                </div>
              </div>
              <DialogFooter>
                <Button 
                  type="button" 
                  class="bg-teal-600 hover:bg-teal-700" 
                  :disabled="passwordLoading || passwordForm.newPassword.length < 8"
                  @click="changePassword"
                >
                  <span v-if="passwordLoading" class="animate-spin border-2 border-white border-t-transparent rounded-full w-4 h-4 mr-2"></span>
                  Zapisz nowe hasło
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </div>

        <div class="pt-4">
          <Button type="submit" class="bg-teal-600 hover:bg-teal-700 text-white font-bold h-12 px-8" :disabled="loading">
            <span v-if="loading" class="animate-spin border-2 border-white border-t-transparent rounded-full w-5 h-5 mr-2"></span>
            Zapisz zmiany konta
          </Button>
        </div>
      </form>

    </CardContent>
  </Card>
</template>
