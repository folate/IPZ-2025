<script setup>
import LandingHeader from "./landing/LandingHeader.vue";
import SettingsMailNotifs from "./SettingsComponents/SettingsMailNotifs.vue";
import SettingsDeafultOrderMethods from "./SettingsComponents/SettingsDeafultOrderMethods.vue";
import * as vue from "vue";

const Current = vue.ref("mail");
function changeCurrent(tab) {
  Current.value = tab;
}
</script>
<template>
  <main>
    <LandingHeader />
    <div :class="SettingsContent">
      <div :class="SettingsTab">
        <ul>
          <li
            v-on:click="changeCurrent('mail')"
            :class="{ active: Current === 'mail' }"
          >
            <span>Email notifications</span>
          </li>
          <li
            v-on:click="changeCurrent('deafultOrders')"
            :class="{ active: Current === 'deafultOrders' }"
          >
            <span>Default Settings</span>
          </li>
        </ul>
      </div>
      <div v-if="Current === 'mail'" :class="SettingsBody">
        <SettingsMailNotifs />
      </div>
      <div v-else-if="Current === 'deafultOrders'" :class="SettingsBody">
        <SettingsDeafultOrderMethods />
      </div>
    </div>
  </main>
</template>
