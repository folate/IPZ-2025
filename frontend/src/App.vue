<script setup>
  import { ref } from 'vue'
import { RouterLink, RouterView } from 'vue-router'
import HelloWorld from './components/HelloWorld.vue'
import LoginModal from './components/LoginModal.vue'
import RegisterModal from './components/RegisterModal.vue'
const showLogin = ref(false)
const showRegister = ref(false)
const handleSwitch = () => {
  showLogin.value = false;
  showRegister.value = true;
}
</script>

<template>
  <header>
    <img alt="Vue logo" class="logo" src="@/assets/logo.svg" width="125" height="125" />

    <div class="wrapper">
      <HelloWorld msg="You did it!" />

      <nav>
        <RouterLink to="/">Home</RouterLink>
        <RouterLink to="/about">About</RouterLink>
        <button id="logIn-link" @click="showLogin = true">Open Login</button>
      </nav>
    </div>
  </header>

  <RouterView />
  <LoginModal :isOpen="showLogin" @close="showLogin = false" @switchToRegister="handleSwitch"/>

  <RegisterModal :isOpen="showRegister" @close="showRegister = false"/>
</template>

<style scoped>
header {
  line-height: 1.5;
  max-height: 100vh;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

nav {
  width: 100%;
  font-size: 12px;
  text-align: center;
  margin-top: 2rem;
}

nav a.router-link-exact-active{
  color: var(--color-text);
}

nav a.router-link-exact-active:hover{
  background-color: transparent;
}

nav a {
  display: inline-block;
  padding: 0 1rem;
  border-left: 1px solid var(--color-border);
}

nav a:first-of-type {
  border: 0;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }

  nav {
    text-align: left;
    margin-left: -1rem;
    font-size: 1rem;

    padding: 1rem 0;
    margin-top: 1rem;
  }
}
button#logIn-link{
  background: none;
  border: none;
  font: inherit;
  cursor: pointer;
  padding: 0 1rem;
  color: hsla(160, 100%, 37%, 1);
}
</style>
