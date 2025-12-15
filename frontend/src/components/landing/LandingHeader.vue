<script setup>
import Container from "../ui/Container.vue"
import IconButton from "../ui/IconButton.vue"
</script>

<template>
  <header class="top">
    <Container class="row">
      <div class="logo">LOGO</div>

      <div class="actions">
        <IconButton label="Cart">
        <img class="ico" src="/icons/cart.png" alt="" />
        </IconButton>

        <IconButton label="LogIn">
        <img class="ico" src="/icons/login.png" alt="" />
        </IconButton>
      </div>
    </Container>
  </header>
</template>

<style scoped>
.top
{
  background: #e9e9e9;
  border-bottom: 1px solid rgba(0,0,0,.12);
}

.row
{
  height: 56px;
  display:flex;
  align-items:center;
  justify-content:space-between;
}

.logo
{
  font-weight: 900;
  letter-spacing: .6px;
  color: rgba(0,0,0,.65);
}

.actions
{
  display:flex;
  gap: 10px;
}

.ico
{
  width: 30px;
  height: 30px;
  object-fit: contain;
  opacity: .85;
}

</style>
