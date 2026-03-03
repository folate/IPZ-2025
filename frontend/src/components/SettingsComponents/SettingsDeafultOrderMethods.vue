<script setup>
import { string } from "@vueform/vueform";
import * as vue from "vue";
const details = vue.ref({
  ShippingAddress: "",
  BillingAdress: "",
  PreferredPaymentMethod: "PayPal",
});
const previousDetails = vue.ref({});
const isEditing = vue.ref(false);
function changeSettings() {
  isEditing.value = true;
}
async function savePreferences() {
  const payload = {
    shippingAddress:
      details.value.ShippingAddress.trim() ||
      previousDetails.value.ShippingAddress,
    billingAddress:
      details.value.BillingAdress.trim() || previousDetails.value.BillingAdress,
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
    fetchProfileInfo();
  } catch (err) {
    console.error("Profile Error:", err);
  }
}
async function fetchProfileInfo() {
  try {
    const response = await fetch("/api/Buyer/me");

    if (!response.ok) {
      const errorText = await response.text();
      console.error("Server Error:", response.status, errorText);
      return;
    }
    const result = await response.json();
    const data = {
      ShippingAddress: result.shippingAddress,
      BillingAdress: result.billingAddress,
      PreferredPaymentMethod: result.preferredPaymentMethod,
    };
    details.value = { ...data };
    previousDetails.value = { ...data };
    console.log("Success");
  } catch (err) {
    console.error("Profile Error:", err);
  }
}
function CancelSave() {
  details.value = { ...previousDetails.value };
  isEditing.value = false;
}
vue.onMounted(fetchProfileInfo);
</script>
<template>
  <main>
    <div class="Settings-content">
      <span>Preffered order info:</span>
      <div class="Setting-group">
        <input
          v-if="isEditing"
          type="text"
          v-model="details.ShippingAddress"
          :placeholder="details.ShippingAddress"
        />
        <p v-else>Shipping Adress: {{ details.ShippingAddress }}</p>
        <p v-if="!details.ShippingAddress.trim()" class="error-subtext">
          Field is empty. Reverting to: {{ previousDetails.ShippingAddress }}
        </p>
      </div>
      <div class="Setting-group">
        <input
          v-if="isEditing"
          type="text"
          v-model="details.BillingAdress"
          :placeholder="details.BillingAdress"
        />
        <p v-else>Billing Adress: {{ details.BillingAdress }}</p>
        <p v-if="!details.BillingAdress.trim()" class="error-subtext">
          Field is empty. Reverting to: {{ previousDetails.BillingAdress }}
        </p>
      </div>
      <div class="Setting-group">
        <select
          :name="details.PreferredPaymentMethod"
          v-if="isEditing"
          v-model="details.PreferredPaymentMethod"
        >
          <option value="PayPal">PayPal</option>
          <option value="Debit">Debit</option>
          <option value="Blik">Blik</option>
        </select>
        <p v-else>
          Preferred Payment Method: {{ details.PreferredPaymentMethod }}
        </p>
      </div>
      <button v-if="isEditing" v-on:click="savePreferences()">Save</button>
      <button v-else v-on:click="changeSettings()">Change</button>
      <button v-if="isEditing" v-on:click="CancelSave()">Cancel</button>
    </div>
  </main>
</template>
