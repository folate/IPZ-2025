<script setup>
import { computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useAuth } from "../../stores/auth";
import LoginModal from "../LoginModal.vue";
import RegisterModal from "../RegisterModal.vue";
import { ref as VueRef } from "vue";
import AdForm from "../AdForm.vue";
const showAdForm = VueRef(false);

const showLogin = VueRef(false);
const showRegister = VueRef(false);

const router = useRouter();
const { isLoggedIn, initAuth, logout } = useAuth();

onMounted(async () => {
  await initAuth();
});

const handleSwitch = () => {
  showLogin.value = false;
  showRegister.value = true;
};

async function closeLoginAndRefresh() {
  showLogin.value = false;
  await initAuth();
}

async function closeRegisterAndRefresh() {
  showRegister.value = false;
  await initAuth();
}

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
      src: "/icons/adform.png", // 2. Ensure this matches your filename
      onClick: () => (showAdForm.value = true), // 3. Open the modal
    },
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
    @close="closeLoginAndRefresh"
    @switchToRegister="handleSwitch"
  />

  <RegisterModal :isOpen="showRegister" @close="closeRegisterAndRefresh" />
  <AdForm :isOpen="showAdForm" @close="showAdForm = false" />
</template>
