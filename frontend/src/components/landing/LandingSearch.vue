<script setup>
import Container from "../ui/Container.vue";
import { ref, onBeforeUnmount } from "vue";

const query = ref("");
let t = null;

function emitSearch(value) {
  window.dispatchEvent(new CustomEvent("search:changed", { detail: value }));
}

function onInput(e) {
  query.value = e.target.value;

  if (t) clearTimeout(t);
  t = setTimeout(() => {
    emitSearch(query.value);
  }, 300);
}

function onSubmit(e) {
  e.preventDefault();
  emitSearch(query.value);
}

onBeforeUnmount(() => {
  if (t) clearTimeout(t);
});
</script>

<template>
  <section class="wrap">
    <Container>
      <form class="search" @submit="onSubmit">
        <input :value="query" placeholder="Szukaj..." @input="onInput" />
        <button class="icon" type="submit" aria-label="Szukaj">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
            <path
              d="M10.5 18a7.5 7.5 0 1 1 0-15 7.5 7.5 0 0 1 0 15Z"
              stroke="currentColor"
              stroke-width="2"
            />
            <path d="M16.5 16.5 21 21" stroke="currentColor" stroke-width="2" />
          </svg>
        </button>
      </form>
    </Container>
  </section>
</template>
