<script setup>
import { ref } from "vue";
import SettingsMailNotifs from "./SettingsComponents/SettingsMailNotifs.vue";
import SettingsDeafultOrderMethods from "./SettingsComponents/SettingsDeafultOrderMethods.vue";
import SettingsAccount from "./SettingsComponents/SettingsAccount.vue";

import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import Container from "@/components/ui/Container.vue";

const currentTab = ref("account");
</script>

<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20">
    <Container>
      <div class="mt-8 w-full flex flex-col gap-6">
        <div class="animate-in fade-in slide-in-from-left duration-500 fill-mode-both">
          <h1 class="text-3xl font-extrabold text-zinc-900 dark:text-zinc-50">Ustawienia Konta</h1>
          <p class="text-zinc-500 dark:text-zinc-400 mt-1">Zarządzaj powiadomieniami i domyślnymi danymi zamówień.</p>
        </div>

        <Tabs v-model="currentTab" class="w-full animate-in fade-in slide-in-from-bottom-4 delay-150 duration-500 fill-mode-both">
          <TabsList class="w-full justify-start h-auto flex-wrap bg-transparent border-b border-zinc-200 dark:border-zinc-800 p-0 rounded-none gap-8">
            <TabsTrigger 
              value="account" 
              class="rounded-none border-b-2 border-transparent px-0 py-3 text-lg font-semibold text-zinc-500 dark:text-zinc-400 data-[state=active]:border-teal-600 data-[state=active]:text-teal-700 dark:data-[state=active]:text-teal-400 data-[state=active]:shadow-none data-[state=active]:bg-transparent focus-visible:ring-0 cursor-pointer"
            >
              Konto i Bezpieczeństwo
            </TabsTrigger>
            <TabsTrigger 
              value="mail" 
              class="rounded-none border-b-2 border-transparent px-0 py-3 text-lg font-semibold text-zinc-500 dark:text-zinc-400 data-[state=active]:border-teal-600 data-[state=active]:text-teal-700 dark:data-[state=active]:text-teal-400 data-[state=active]:shadow-none data-[state=active]:bg-transparent focus-visible:ring-0 cursor-pointer"
            >
              Powiadomienia Email
            </TabsTrigger>
            <TabsTrigger 
              value="defaultOrders" 
              class="rounded-none border-b-2 border-transparent px-0 py-3 text-lg font-semibold text-zinc-500 dark:text-zinc-400 data-[state=active]:border-teal-600 data-[state=active]:text-teal-700 dark:data-[state=active]:text-teal-400 data-[state=active]:shadow-none data-[state=active]:bg-transparent focus-visible:ring-0 cursor-pointer"
            >
              Domyślne ustawienia zakupów
            </TabsTrigger>
          </TabsList>

          <TabsContent value="account" class="pt-6">
            <SettingsAccount />
          </TabsContent>
          <TabsContent value="mail" class="pt-6">
            <SettingsMailNotifs />
          </TabsContent>
          <TabsContent value="defaultOrders" class="pt-6">
            <SettingsDeafultOrderMethods />
          </TabsContent>
        </Tabs>
      </div>
    </Container>
  </div>
</template>
