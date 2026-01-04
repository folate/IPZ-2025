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

<style scoped>
.card {
  border: 1px solid rgba(0,0,0,.10);
  border-radius: 10px;
  overflow: hidden;
  background: #e3e3e3;
  cursor: pointer;
}

.card:focus {
  outline: 2px solid rgba(0,0,0,.35);
  outline-offset: 3px;
}

.media{
  position: relative;
  height: 150px;
  background: #cfcfcf;
  border: 3px solid rgba(0,0,0,.18);
}

.priceTop{
  position:absolute;
  top: 8px;
  left: 10px;
  font-weight: 900;
  color: rgba(0,0,0,.55);
}

.stars{
  position:absolute;
  top: 8px;
  right: 10px;
  font-size: 16px;
  letter-spacing: 2px;
}

.stars span{ color: rgba(0,0,0,.18); }
.stars span.on{ color: rgba(0,0,0,.35); }

.title{
  position:absolute;
  left: 12px;
  bottom: 34px;
  font-weight: 900;
  color: rgba(0,0,0,.55);
}

.meta{
  position:absolute;
  left: 12px;
  right: 12px;
  bottom: 10px;
  display:flex;
  justify-content:space-between;
  color: rgba(0,0,0,.45);
  font-weight: 800;
}
.price{ letter-spacing: .4px; }
</style>
