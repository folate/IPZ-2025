<script setup>
import { computed } from "vue"
import { useAuth } from "../../stores/auth"
import { ROLES } from "../../auth/roles"

const { isLoggedIn, setMockRole, logout } = useAuth()

const iconItems = computed(() => {
  //Niezalogowany
  if (!isLoggedIn.value) 
  {
    return [
      { key: "cart", src: "/icons/cart.png", onClick: () => {} },
      // DEV: klik login = ustaw rolę USER (żebyś mógł testować bez backendu)
      { key: "login", src: "/icons/login.png", onClick: () => setMockRole(ROLES.USER) },
    ]
  }

  //Zalogowany
  return [
    { key: "upload", src: "/icons/upload.png", onClick: () => {} },
    { key: "favourites", src: "/icons/favourites.png", onClick: () => {} },
    { key: "notfulfilled", src: "/icons/notfulfilled.png", onClick: () => {} },
    { key: "user", src: "/icons/user.png", onClick: () => {} },
    { key: "logout", src: "/icons/logout.png", onClick: () => logout() },
    { key: "cart", src: "/icons/cart.png", onClick: () => {} },
  ]
})
</script>

<template>
  <header class="landingHeader">
    <div class="logo">LOGO</div>

    <div class="header-icons">
      <button
        v-for="item in iconItems"
        :key="item.key"
        class="icon-box"
        type="button"
        @click="item.onClick"
      >
        <img :src="item.src" :alt="item.key" />
      </button>
    </div>
  </header>
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

.header-icons 
{
  display: flex;
  gap: 10px;
}

.icon-box 
{
  width: 38px;
  height: 38px;
  background: #f2f2f2;
  border-radius: 10px;
  border: 1px solid #cfcfcf;

  display: grid;
  place-items: center;

  cursor: pointer;
  padding: 0;
}

.icon-box:hover 
{
  background: #e6e6e6;
}

.icon-box img 
{
  width: 30px;
  height: 30px;
  object-fit: contain;
}
</style>
