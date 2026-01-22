<script setup>
import { computed } from "vue";

const props = defineProps({
  offer: { type: Object, required: true },
});

const user = computed(() => props.offer?.freelancer || "USER");
const title = computed(() => props.offer?.title || "TYTUŁ");

const price = computed(() => {
  const gigs = props.offer?.gigs;
  if (!Array.isArray(gigs) || gigs.length === 0) return "0 zł";

  const prices = gigs.map((g) => Number(g?.price)).filter(Number.isFinite);

  if (prices.length === 0) return "0 zł";

  const min = Math.min(...prices);
  const max = Math.max(...prices);

  return min === max
    ? `${min.toFixed(0)} zł`
    : `${min.toFixed(0)}-${max.toFixed(0)} zł`;
});
</script>

<template>
  <article class="offerCard">
    <div class="offerStars" aria-label="rating">★★★★★</div>

    <h3 class="offerTitle">{{ title }}</h3>

    <div class="offerDivider"></div>

    <div class="offerMeta">
      <span class="offerUser">{{ user }}</span>
      <span class="offerPrice">{{ price }}</span>
    </div>
  </article>
</template>
