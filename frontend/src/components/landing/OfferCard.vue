<script setup>
const props = defineProps({
  title: { type: String, default: "TYTUŁ" },
  user: { type: String, default: "USER" },
  price: { type: String, default: "CENA" },
  stars: { type: Number, default: 3 }, // 0..5
  showPriceTop: { type: Boolean, default: false },
})

const emit = defineEmits(["click"])
</script>

<template>
  <article class="card" role="button" tabindex="0"
    @click="emit('click')"
    @keydown.enter.prevent="emit('click')"
    @keydown.space.prevent="emit('click')"
  >
    <div class="media">
      <div v-if="showPriceTop" class="priceTop">CENA</div>

      <div class="stars" aria-label="Ocena">
        <span v-for="i in 5" :key="i" :class="{ on: i <= stars }">★</span>
      </div>

      <div class="title">{{ title }}</div>
      <div class="meta">
        <span>{{ user }}</span>
        <span class="price">{{ price }}</span>
      </div>
    </div>
  </article>
</template>

