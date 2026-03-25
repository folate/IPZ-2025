<script setup>
import { computed } from "vue";
import { Star } from "lucide-vue-next";
import { Card, CardContent, CardFooter } from "@/components/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";

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
const category = computed(() => props.offer?.category || "CATEGORY");

const displayImage = computed(() => {
  if (props.offer.image) return props.offer.image;
  if (props.offer.photos && Array.isArray(props.offer.photos) && props.offer.photos.length > 0) {
    const p = props.offer.photos.find(p => p.isMain) || props.offer.photos[0];
    return p ? p.url : null;
  }
  return null;
});

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
  <Card class="group overflow-hidden rounded-2xl border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-950 transition-all duration-300 hover:-translate-y-1 hover:shadow-xl hover:shadow-teal-900/5 hover:border-teal-200 dark:hover:border-teal-900/50 flex flex-col h-full cursor-pointer pb-6 pt-0">
    <div class="h-44 w-full bg-zinc-100 dark:bg-zinc-900 overflow-hidden relative">
      <img v-if="displayImage" :src="displayImage" :alt="title" class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105" />
      <div v-else class="h-full w-full flex items-center justify-center text-zinc-300 dark:text-zinc-700 bg-gradient-to-br from-zinc-100 to-zinc-200 dark:from-zinc-900 dark:to-zinc-800">
        <span class="text-xs font-bold uppercase tracking-widest opacity-50">Brak Zdjęcia</span>
      </div>
    </div>
    
    <CardContent class="flex-grow p-4 flex flex-col group-hover:bg-zinc-50/50 dark:group-hover:bg-zinc-900/30 transition-colors duration-300">
      <div class="flex items-center gap-1 ">
        <span class="text-xs text-zinc-400 font-normal">{{ category }}</span>
      </div>

      <h3 class="font-semibold text-lg leading-tight text-zinc-900 dark:text-zinc-50 line-clamp-2 mb-2 group-hover:text-teal-600 dark:group-hover:text-teal-400 transition-colors">
        {{ title }}
      </h3>
    </CardContent>

    <CardFooter class="p-4 pt-4 mt-auto border-t border-zinc-100 dark:border-zinc-800/50 flex items-center justify-between">
      <div class="flex items-center gap-2">
        <Avatar class="h-8 w-8 border border-zinc-200 dark:border-zinc-800">
          <AvatarImage src="" />
          <AvatarFallback class="bg-teal-100 dark:bg-teal-900/30 text-teal-700 dark:text-teal-400 text-xs font-bold">{{ user.substring(0,2).toUpperCase() }}</AvatarFallback>
        </Avatar>
        <span class="text-sm font-medium text-zinc-600 dark:text-zinc-400 truncate max-w-[100px]">{{ user }}</span>
      </div>
      <div class="text-right flex flex-col items-end">
        <span class="text-[10px] text-zinc-400 uppercase font-bold tracking-wider leading-none mb-1">Cena od</span>
        <span class="font-bold text-teal-600 dark:text-teal-400 leading-none">{{ price }}</span>
      </div>
    </CardFooter>
  </Card>
</template>
