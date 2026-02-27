<script setup>
import { computed } from "vue";

const props = defineProps({
  offer: { type: Object, required: true },
});

const user = computed(() => {
  return (
    props.offer?.freelancer ||
    props.offer?.buyerName ||
    props.offer?.buyer ||
    "USER"
  );
});

const title = computed(() => props.offer?.title || "TYTUŁ");

const price = computed(() => {
  const gigs = props.offer?.gigs;
  if (Array.isArray(gigs) && gigs.length) {
    const prices = gigs.map((g) => Number(g?.price)).filter(Number.isFinite);
    if (!prices.length) return "0 zł";

    const min = Math.min(...prices);
    const max = Math.max(...prices);

    return min === max
      ? `${min.toFixed(0)} zł`
      : `${min.toFixed(0)}-${max.toFixed(0)} zł`;
  }

  const budget = props.offer?.budget;
  if (budget != null && budget !== "") {
    const n = Number(budget);
    return Number.isFinite(n) ? `${n.toFixed(0)} zł` : "—";
  }

  return "—";
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
