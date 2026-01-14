<script setup>
import { computed, ref, watch } from "vue";
import { useRouter } from "vue-router";
import { useAuth } from "../../stores/auth";
import LoginModal from "../LoginModal.vue";
import RegisterModal from "../RegisterModal.vue";
import AdForm from "../AdForm.vue";
const showAdForm = ref(false);
const router = useRouter();
const { isLoggedIn, logout } = useAuth();

const showLogin = ref(false);
const showRegister = ref(false);
const handleAdForm = () => {
  showAdForm.value = true;
};

const handleSwitch = () => {
  showLogin.value = false;
  showRegister.value = true;
};

watch(isLoggedIn, (v) => {
  if (v) {
    showLogin.value = false;
    showRegister.value = false;
  }
});

const iconItems = computed(() => {
  if (!isLoggedIn.value) {
    return [
      { key: "cart", src: "/icons/cart.png", onClick: () => {} },
      {
        key: "login",
        src: "/icons/login.png",
        onClick: () => (showLogin.value = true),
      },
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
    {
      key: "ad-upload",
      src: "/icons/upload.png",
      onClick: () => handleAdForm(),
    },
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
    @close="showLogin = false"
    @switchToRegister="handleSwitch"
  />

  <RegisterModal :isOpen="showRegister" @close="showRegister = false" />
  <AdForm :isOpen="showAdForm" @close="showAdForm = false" />
</template>
