<script setup>
import { computed, onMounted, ref as VueRef } from "vue";
import { useRouter } from "vue-router";
import { useAuth } from "../../stores/auth";
import { ROLES } from "../../auth/roles";
import AdForm from "../AdForm.vue";
import {
  ShoppingCart,
  LogIn,
  PlusCircle,
  FilePlus,
  Heart,
  ListX,
  User,
  LogOut,
} from "lucide-vue-next";

const showAdForm = VueRef(false);

const router = useRouter();
const { isLoggedIn, initAuth, logout, hasRole } = useAuth();

onMounted(async () => {
  await initAuth();
});

const goToProfile = () => {
  if (hasRole(ROLES.SELLER)) return router.push("/seller/profile");
  return router.push("/buyer/profile");
};

const iconItems = computed(() => {
  if (!isLoggedIn.value) {
    return [
      { key: "cart", icon: ShoppingCart, onClick: () => {} },
      {
        key: "login",
        icon: LogIn,
        onClick: () => router.push("/login"),
      },
    ];
  }

  const items = [];

  if (hasRole(ROLES.SELLER)) {
    items.push({
      key: "adform",
      label: "Utwórz Ogłoszenie Usługi",
      icon: PlusCircle,
      onClick: () => (showAdForm.value = true),
    });
  }

  if (hasRole(ROLES.BUYER)) {
    items.push({
      key: "upload",
      label: "Dodaj Zlecenie Usługi",
      icon: FilePlus,
      onClick: () => router.push("/buyer/ad"),
    });
  }

  items.push(
    {
      key: "favourites",
      label: "Ulubione",
      icon: Heart,
      onClick: () => router.push("/liked"),
    },
    {
      key: "notfulfilled",
      label: "Niespełnione",
      icon: ListX,
      onClick: () => {},
    },
    {
      key: "user",
      label: "Profil",
      icon: User,
      onClick: goToProfile,
    },
    {
      key: "logout",
      label: "Wyloguj",
      icon: LogOut,
      onClick: async () => {
        await logout();
        router.push("/");
      },
    },
    {
      key: "cart",
      label: "Koszyk",
      icon: ShoppingCart,
      onClick: () => {},
    },
  );

  return items;
});
</script>

<template>
  <header
    class="sticky top-0 z-50 w-full border-b bg-background/80 backdrop-blur-md"
  >
    <div
      class="w-full max-w-7xl mx-auto flex h-16 items-center justify-between px-4 sm:px-6 lg:px-8"
    >
      <div
        class="text-2xl font-black tracking-tighter text-teal-600 dark:text-teal-500 cursor-pointer"
        @click="router.push('/')"
      >
        LOGO
      </div>

      <div class="flex items-center gap-2 sm:gap-4">
        <button
          v-for="item in iconItems"
          :key="item.key"
          class="flex h-10 w-10 items-center justify-center rounded-full text-zinc-600 dark:text-zinc-400 hover:text-teal-600 dark:hover:text-teal-400 hover:bg-zinc-100 dark:hover:bg-zinc-800 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-teal-500"
          type="button"
          @click="item.onClick"
          :title="item.label"
        >
          <component :is="item.icon" class="h-5 w-5" />
        </button>
      </div>
    </div>
  </header>

  <AdForm :isOpen="showAdForm" @close="showAdForm = false" />
</template>
