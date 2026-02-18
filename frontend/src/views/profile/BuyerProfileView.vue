<script setup>
import LandingHeader from "../../components/landing/LandingHeader.vue";
import { reactive } from "vue";
vue.onMounted(() => {
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
    ProfileInfo.JoinDate = result.joinedDate.slice(0, 10) ?? "Never";
    ProfileInfo.TotalOrders = result.totalOrders;
    ProfileInfo.LastOrder = result.lastorderDate.slice(0, 10) ?? "No orders";
    console.log("Success");
    emit("close");
  } catch (err) {
    console.error("Profile Error:", err);
  }
};
</script>
<template>
  <main>
    <LandingHeader />
    Hi {{ ProfileInfo.firstName }} {{ ProfileInfo.lastName }}!<br />
    Joined date:{{ ProfileInfo.JoinDate }}<br />
    Total Orders: {{ ProfileInfo.totalOrders }}<br />
    Last Order Date:{{ ProfileInfo.lastorderDate }}<br />
    In Progress: <br />
    Finished:<br />
  </main>
</template>
