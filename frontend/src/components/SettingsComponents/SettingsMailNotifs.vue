<script setup>
import * as vue from "vue";
const notifications = vue.ref({
  updates: false,
  alerts: false,
  reminders: false,
});
function savePreferences() {
  console.log(notifications);
}
</script>
<template>
  <main>
    <div class="Settings-content">
      <span>I want to recieve:</span>
      <div class="checkbox-group">
        <input
          name="Updates"
          type="checkbox"
          :value="true"
          id="UpdateNotif"
          v-model="notifications.updates"
        />
        <label for="UpdateNotif" class="no-select"> Updates</label>
      </div>
      <div class="checkbox-group">
        <input
          name="Alerts"
          type="checkbox"
          :value="true"
          id="AlertNotif"
          v-model="notifications.alerts"
        />
        <label for="AlertNotif" class="no-select"> Alerts</label>
      </div>
      <div class="checkbox-group">
        <input
          name="Reminder"
          type="checkbox"
          :value="true"
          id="ReminderNotif"
          v-model="notifications.reminders"
        />
        <label for="ReminderNotif" class="no-select"> Reminders</label>
      </div>
      <button v-on:click="savePreferences()">Save Prefrences</button>
    </div>
  </main>
</template>
