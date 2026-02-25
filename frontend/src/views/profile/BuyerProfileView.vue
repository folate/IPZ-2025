<script setup>
import LandingHeader from "../../components/landing/LandingHeader.vue";
import * as vue from "vue";
import { useRouter } from "vue-router";
const router = useRouter();
vue.onMounted(() => {
  fetchProfileInfo();
  fetchOrders();
});
const ProfileInfo = vue.reactive({
  firstName: "",
  lastName: "",
  JoinDate: "",
  TotalOrders: null,
  LastOrder: "",
});
const fetchOrders = async () => {};
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
  <main>
    <LandingHeader />
    <button v-on:click="router.push('/buyer/profile/settings')">
      Settings!
    </button>
    <p>Hi {{ ProfileInfo.firstName }} {{ ProfileInfo.lastName }}!</p>
    <p>Joined date:{{ ProfileInfo.JoinDate }}</p>
    <p>Total Orders: {{ ProfileInfo.TotalOrders }}</p>
    <p>Last Order Date:{{ ProfileInfo.LastOrder }}</p>
    <div class="InProgress">
      In Progress:
      <div v-for="value in source"></div>
    </div>
    <p>Finished:</p>
  </main>
</template>
