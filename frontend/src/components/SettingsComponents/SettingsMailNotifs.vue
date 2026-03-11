<script setup>
import { ref } from "vue";
import { Checkbox } from "@/components/ui/checkbox";
import { Button } from "@/components/ui/button";

const notifications = ref({
  updates: false,
  alerts: false,
  reminders: false,
});

function savePreferences() {
  console.log("Saving Email Preferences", notifications.value);
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <div class="flex flex-col gap-1">
      <h2 class="text-xl font-bold text-zinc-900 dark:text-zinc-50">Powiadomienia Email</h2>
      <p class="text-zinc-500 dark:text-zinc-400 text-sm">Wybierz, jakie informacje chcesz otrzymywać na swój adres email.</p>
    </div>

    <div class="flex flex-col gap-4">
      
      <!-- Updates -->
      <div class="flex items-start space-x-3">
        <Checkbox id="updateNotif" v-model:checked="notifications.updates" class="mt-1" />
        <div class="flex flex-col gap-1 leading-none">
          <label for="updateNotif" class="text-sm font-medium leading-none cursor-pointer text-zinc-900 dark:text-zinc-300">
            Aktualizacje
          </label>
          <p class="text-sm text-zinc-500 dark:text-zinc-400">
            Informacje o nowych funkcjach i zmianach w platformie.
          </p>
        </div>
      </div>

      <!-- Alerts -->
      <div class="flex items-start space-x-3">
        <Checkbox id="alertNotif" v-model:checked="notifications.alerts" class="mt-1" />
        <div class="flex flex-col gap-1 leading-none">
          <label for="alertNotif" class="text-sm font-medium leading-none cursor-pointer text-zinc-900 dark:text-zinc-300">
            Alerty bezpieczeństwa
          </label>
          <p class="text-sm text-zinc-500 dark:text-zinc-400">
            Ważne informacje dotyczące logowań i stanu Twojego konta.
          </p>
        </div>
      </div>

      <!-- Reminders -->
      <div class="flex items-start space-x-3">
        <Checkbox id="reminderNotif" v-model:checked="notifications.reminders" class="mt-1" />
        <div class="flex flex-col gap-1 leading-none">
          <label for="reminderNotif" class="text-sm font-medium leading-none cursor-pointer text-zinc-900 dark:text-zinc-300">
            Przypomnienia
          </label>
          <p class="text-sm text-zinc-500 dark:text-zinc-400">
            Przypomnienia o nieprzeczytanych wiadomościach i aktywnych zleceniach.
          </p>
        </div>
      </div>

    </div>

    <div class="pt-4 border-t border-zinc-200 dark:border-zinc-800">
      <Button @click="savePreferences" class="bg-teal-600 hover:bg-teal-700 text-white font-medium">
        Zapisz Preferencje
      </Button>
    </div>
  </div>
</template>
