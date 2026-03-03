<script setup>
import { ref, reactive, onMounted } from "vue";
import { useRouter } from "vue-router";
import LandingHeader from "@/components/landing/LandingHeader.vue";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

const router = useRouter();

onMounted(() => {
  fetchProfileInfo();
  // fetchOrders();
});

const ProfileInfo = reactive({
  firstName: "",
  lastName: "",
  JoinDate: "",
  TotalOrders: null,
  LastOrder: "",
});

// const source = ref([]);

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
    ProfileInfo.JoinDate = result.joinedDate?.slice(0, 10) ?? "Never";
    ProfileInfo.TotalOrders = result.totalOrders;
    ProfileInfo.LastOrder = result.lastOrderDate?.slice(0, 10) ?? "No orders";
    console.log("Success");
  } catch (err) {
    console.error("Profile Error:", err);
  }
};
</script>

<template>
  <div class="min-h-svh bg-zinc-50 dark:bg-zinc-950 flex flex-col">
    <LandingHeader />
    <div class="flex-1 flex flex-col items-center justify-center p-6 gap-6 w-full max-w-3xl mx-auto">
      <div class="w-full flex justify-end">
        <Button variant="outline" @click="router.push('/buyer/profile/settings')">
          Ustawienia
        </Button>
      </div>

      <Card class="w-full shadow-xl shadow-teal-900/5 border-zinc-200 dark:border-zinc-800">
        <CardContent class="p-10 flex flex-col items-center justify-center text-center gap-4">
          <h1 class="text-3xl font-extrabold text-zinc-900 dark:text-zinc-50">
            Cześć {{ ProfileInfo.firstName }} {{ ProfileInfo.lastName }}!
          </h1>
          <div class="flex flex-col gap-2 text-zinc-600 dark:text-zinc-400 text-lg mt-4">
            <p><strong>Data dołączenia:</strong> {{ ProfileInfo.JoinDate }}</p>
            <p><strong>Liczba zamówień:</strong> {{ ProfileInfo.TotalOrders }}</p>
            <p><strong>Ostatnie zamówienie:</strong> {{ ProfileInfo.LastOrder }}</p>
          </div>
        </CardContent>
      </Card>
      
      <Card class="w-full shadow-xl shadow-teal-900/5 border-zinc-200 dark:border-zinc-800 mt-4">
        <CardContent class="p-8">
          <h2 class="text-2xl font-bold mb-4">W trakcie (In Progress)</h2>
          <div class="text-zinc-500">Brak zamówień w trakcie.</div>
          
          <h2 class="text-2xl font-bold mt-8 mb-4">Zakończone (Finished)</h2>
          <div class="text-zinc-500">Brak zakończonych zamówień.</div>
        </CardContent>
      </Card>

    </div>
  </div>
</template>
