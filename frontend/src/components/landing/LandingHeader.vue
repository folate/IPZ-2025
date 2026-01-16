<script setup>
import { computed, onMounted, ref as VueRef } from "vue";
import { useRouter } from "vue-router";
import { useAuth } from "../../stores/auth";

import LoginModal from "../LoginModal.vue";
import RegisterModal from "../RegisterModal.vue";

const router = useRouter();
const { isLoggedIn, role, initAuth, logout } = useAuth();

const showLogin = VueRef(false);
const showRegister = VueRef(false);

function openLogin() {
  showRegister.value = false;
  showLogin.value = true;
}

function openRegister() {
  showLogin.value = false;
  showRegister.value = true;
}

function handleSwitchToRegister() {
  openRegister();
}

async function handleLoginClose() {
  showLogin.value = false;
  await initAuth();
}

async function handleRegisterClose() {
  showRegister.value = false;
  await initAuth();
}

onMounted(async () => {
  await initAuth();
});

const iconItems = computed(() => {
  if (!isLoggedIn.value) {
    return [
      { key: "cart", src: "/icons/cart.png", onClick: () => {} },
      { key: "login", src: "/icons/login.png", onClick: openLogin },
    ];
  }

  return [
    {
      key: "upload",
      src: "/icons/upload.png",
      onClick: () => router.push("/buyer/ad"),
    },
    { key: "favourites", src: "/icons/favourites.png", onClick: () => {} },
    { key: "notfulfilled", src: "/icons/notfulfilled.png", onClick: () => {} },
    {
      key: "user",
      src: "/icons/user.png",
      onClick: () => router.push("/seller/profile"),
    },
    { key: "logout", src: "/icons/logout.png", onClick: () => logout() },
    { key: "cart", src: "/icons/cart.png", onClick: () => {} },
  ];
});
</script>

<template>
  <header class="landingHeader">
    <div class="logo">LOGO</div>

    <div class="headerIcons">
      <button
        v-for="item in iconItems"
        :key="item.key"
        class="iconBox"
        type="button"
        @click="item.onClick"
      >
        <img :src="item.src" :alt="item.key" class="iconImg" />
      </button>
    </div>
  </header>

  <LoginModal
    :isOpen="showLogin"
    @close="handleLoginClose"
    @switchToRegister="handleSwitchToRegister"
  />

  <RegisterModal :isOpen="showRegister" @close="handleRegisterClose" />
</template>
