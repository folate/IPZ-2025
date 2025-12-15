<script setup>
import Container from "../ui/Container.vue"
</script>

<template>
  <section class="wrap">
    <Container>
      <div class="search">
        <input placeholder="Szukaj..." />
        <div class="icon" aria-hidden="true">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
            <path d="M10.5 18a7.5 7.5 0 1 1 0-15 7.5 7.5 0 0 1 0 15Z" stroke="currentColor" stroke-width="2"/>
            <path d="M16.5 16.5 21 21" stroke="currentColor" stroke-width="2"/>
          </svg>
        </div>
      </div>
    </Container>
  </section>
</template>

<style scoped>
.wrap{ padding: 28px 0 14px; }

.search
{
  position: relative;
  height: 52px;
  border-radius: 18px;
  border: 2px solid rgba(0,0,0,.18);
  background: #f8f8f8;
  display:flex;
  align-items:center;
  padding: 0 14px;
  max-width: 1200px;
}

.search input
{
  width: 100%;
  border: none;
  outline: none;
  background: transparent;
  font-size: 14px;
  padding-right: 34px;
  max-width: 700px; 
}

.icon
{
  position:absolute;
  right: 14px;
  color: rgba(0,0,0,.55);
}
</style>