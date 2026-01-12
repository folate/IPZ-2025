<script setup>
import { computed } from "vue"
import { useRouter } from "vue-router"
import { useAuth } from "../../stores/auth"
import { ROLES } from "../../auth/roles"
import LoginModal from '../LoginModal.vue'
import RegisterModal from '../RegisterModal.vue'
import { ref as VueRef} from 'vue'
const showLogin = VueRef(false)
const showRegister = VueRef(false)
const handleSwitchToRegister = () => {
  showLogin.value = false;
  showRegister.value = true;
}
const handleRegisterSuccess = () => {
  showRegister.value = false;
  showLogin.value = true;
}
const router = useRouter()
const { isLoggedIn, setMockRole, logout } = useAuth()

const iconItems = computed(() => 
{
  //Niezalogowany
  if (!isLoggedIn.value) {
    return [
      { key: "cart", src: "/icons/cart.png", onClick: () => {} },
      {
        key: "login",
        src: "/icons/login.png",
        onClick: () => setMockRole(ROLES.USER), // DEV login
      },
    ]
  }

  //Zalogowany
  return [
    { key: "upload", src: "/icons/upload.png", onClick: () => router.push("/buyer/ad") },
    { key: "favourites", src: "/icons/favourites.png", onClick: () => {} },
    { key: "notfulfilled", src: "/icons/notfulfilled.png", onClick: () => {} },

    {
      key: "user",
      src: "/icons/user.png",
      onClick: () => router.push("/seller/profile"),
    },

    {
      key: "logout",
      src: "/icons/logout.png",
      onClick: () => logout(),
    },
    { key: "cart", src: "/icons/cart.png", onClick: () => {} },
  ]
})
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
      <button id="logIn-link" @click="showLogin = true">Open Login</button>
    </div>
  </header>
  <!-- <LoginModal :isOpen="showLogin" @close="showLogin = false" @switchToRegister="handleSwitch"/>

  <RegisterModal :isOpen="showRegister" @close="showRegister = false"/> -->
  <LoginModal 
  :isOpen="showLogin" 
  @close="showLogin = false" 
  @switchToRegister="handleSwitchToRegister"
/>

<RegisterModal 
  :isOpen="showRegister" 
  @close="showRegister = false" 
  @switchToLogin="handleRegisterSuccess"
/>
</template>