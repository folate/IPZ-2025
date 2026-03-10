<script setup>
import { computed } from "vue";

const props = defineProps({
  offer: { type: Object, required: true },
});

function pickUserName(o) {
  if (!o) return null;

  if (typeof o.freelancer === "string" && o.freelancer.trim())
    return o.freelancer;

  if (typeof o.buyer === "string" && o.buyerName.trim()) return o.buyerName;

  const buyerObj = o.buyer || o.Buyer || null;
  if (buyerObj) {
    if (typeof buyerObj.userName === "string" && buyerObj.userName.trim())
      return buyerObj.userName;
    if (typeof buyerObj.username === "string" && buyerObj.username.trim())
      return buyerObj.username;
    if (typeof buyerObj.login === "string" && buyerObj.login.trim())
      return buyerObj.login;
    if (typeof buyerObj.email === "string" && buyerObj.email.trim())
      return buyerObj.email;
  }

  const freelancerObj =
    o.freelancerObj || o.freelancerUser || o.Freelancer || null;
  if (freelancerObj) {
    if (
      typeof freelancerObj.userName === "string" &&
      freelancerObj.userName.trim()
    )
      return freelancerObj.userName;
    if (
      typeof freelancerObj.username === "string" &&
      freelancerObj.username.trim()
    )
      return freelancerObj.username;
    if (typeof freelancerObj.login === "string" && freelancerObj.login.trim())
      return freelancerObj.login;
    if (typeof freelancerObj.email === "string" && freelancerObj.email.trim())
      return freelancerObj.email;
  }

  if (typeof o.userName === "string" && o.userName.trim()) return o.userName;
  if (typeof o.username === "string" && o.username.trim()) return o.username;
  if (typeof o.login === "string" && o.login.trim()) return o.login;

  return null;
}

const user = computed(() => pickUserName(props.offer) || "USER");
const title = computed(() => props.offer?.title || "TITLE");

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
