<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import LandingHeader from "@/components/landing/LandingHeader.vue";

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
  <main>
    <LandingHeader />

    <section style="padding: 24px">
      <h1 v-if="ad">{{ ad.title }}</h1>

      <p v-if="loading">Ładowanie...</p>
      <p v-else-if="error">{{ error }}</p>

      <div v-else-if="ad">
        <p><strong>Description:</strong> {{ ad.description }}</p>
        <p><strong>Category:</strong> {{ ad.category }}</p>
        <p><strong>Budget:</strong> {{ ad.budget }} zł</p>
        <p><strong>Deadline:</strong> {{ String(ad.deadline).slice(0, 10) }}</p>

        <p v-if="ad.buyerName"><strong>Client:</strong> {{ ad.buyerName }}</p>
      </div>
    </section>
  </main>
</template>
