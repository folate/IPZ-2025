<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import Container from "@/components/ui/Container.vue";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { useAuth } from "@/stores/auth";

const router = useRouter();
const { initAuth } = useAuth();
const loading = ref(false);
const error = ref("");

const form = ref({
  bio: "",
  skills: "",
  hourlyRate: 0,
  portfolioUrl: ""
});

const submitProfile = async () => {
  loading.value = true;
  error.value = "";
  try {
    const res = await fetch("/api/Seller", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(form.value),
    });

    if (res.ok) {
      await initAuth(); // Refresh roles
      router.push("/seller/profile");
    } else {
      const msg = await res.text();
      error.value = msg || "Wystąpił błąd podczas tworzenia profilu.";
    }
  } catch (err) {
    error.value = err.message || "Błąd sieci";
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20">
    
    <Container>
      <div class="mt-8 max-w-2xl mx-auto w-full flex flex-col gap-6">
        <div>
          <h1 class="text-3xl font-extrabold text-zinc-900 dark:text-zinc-50">Dołącz jako Sprzedawca</h1>
          <p class="text-zinc-500 dark:text-zinc-400 mt-1">Uzupełnij szczegóły swojego profesjonalnego profilu.</p>
        </div>

        <Card class="w-full shadow-lg shadow-teal-900/5 border-zinc-200 dark:border-zinc-800">
          <CardHeader class="border-b border-zinc-100 dark:border-zinc-800/50 pb-4">
            <CardTitle>Profil Wykonawcy</CardTitle>
          </CardHeader>
          <CardContent class="p-6 md:p-8 flex flex-col gap-6">
            
            <div v-if="error" class="p-4 bg-red-50 text-red-600 rounded-xl font-medium">
              {{ error }}
            </div>

            <form @submit.prevent="submitProfile" class="flex flex-col gap-5">
              <div class="space-y-2">
                <Label for="bio">Bio</Label>
                <Textarea id="bio" v-model="form.bio" placeholder="Opisz swoje doświadczenie i to co robisz" class="h-24 resize-y" required />
              </div>
              
              <div class="space-y-2">
                <Label for="skills">Umiejętności</Label>
                <Input id="skills" v-model="form.skills" placeholder="np. Vue.js, C#, Projektowanie graficzne" required />
              </div>

              <div class="grid grid-cols-2 gap-4">
                <div class="space-y-2">
                  <Label for="hourlyRate">Stawka godzinowa (PLN)</Label>
                  <Input id="hourlyRate" type="number" min="0" step="0.01" v-model="form.hourlyRate" required />
                </div>
                
                <div class="space-y-2">
                  <Label for="portfolioUrl">Link do portfolio</Label>
                  <Input id="portfolioUrl" type="url" v-model="form.portfolioUrl" placeholder="https://..." />
                </div>
              </div>

              <div class="pt-4">
                <Button type="submit" class="w-full bg-teal-600 hover:bg-teal-700 text-white font-bold h-12" :disabled="loading">
                  <span v-if="loading" class="animate-spin border-2 border-white border-t-transparent rounded-full w-5 h-5 mr-2"></span>
                  Utwórz profil sprzedawcy
                </Button>
              </div>
            </form>

          </CardContent>
        </Card>
      </div>
    </Container>
  </div>
</template>
