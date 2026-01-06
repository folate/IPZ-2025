<script setup>
import { computed } from "vue"
import { useRouter } from "vue-router"
import { useAuth } from "../../stores/auth"
import { ROLES } from "../../auth/roles"

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
    </div>
  </header>
  <LoginModal :isOpen="showLogin" @close="showLogin = false" @switchToRegister="handleSwitch"/>

  <RegisterModal :isOpen="showRegister" @close="showRegister = false"/>
</template>

<style scoped>
.landingHeader 
{
  display: flex;
  justify-content: space-between;
  align-items: center;

  padding: 14px 24px;
  background: #f2f2f2;
  border-bottom: 1px solid #cfcfcf;
}

.logo 
{
  font-weight: 800;
  font-size: 20px;
}

.headerIcons 
{
  display: flex;
  gap: 10px;
}

.iconBox 
{
  width: 46px;
  height: 46px;

  display: grid;
  place-items: center;

  background: #f2f2f2;
  border: 1px solid #cfcfcf;
  border-radius: 12px;

  cursor: pointer;
  padding: 0;
}

.iconBox:hover 
{
  background: #e6e6e6;
}

.iconImg 
{
  width: 28px;
  height: 28px;
  object-fit: contain;
}
</style>
