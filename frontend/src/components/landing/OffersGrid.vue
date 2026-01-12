<script setup>
import { useRouter } from "vue-router"
import Container from "../ui/Container.vue"

const router = useRouter()

function onOfferClick(offer) 
{
  //na razie każda oferta -> buyer profile
  router.push("/buyer/profile")
}

const offers = [
  { id: 1, variant: "A", stars: 3 },
  { id: 2, variant: "B", stars: 2 },
  { id: 3, variant: "C", stars: 3 },
  { id: 4, variant: "B", stars: 4 },
  { id: 5, variant: "C", stars: 3 },
  { id: 6, variant: "A", stars: 3 },
  { id: 7, variant: "B", stars: 2 },
  { id: 8, variant: "C", stars: 4 },
]
</script>

<template>
  <section class="wrap">
    <Container>
      <div class="grid">
        <button
          v-for="o in offers"
          :key="o.id"
          class="card"
          type="button"
          @click="onOfferClick(o)"
        >
          <div class="box" :class="`v-${o.variant}`">
            <div class="stars">
              <span v-for="i in 5" :key="i" :class="{ on: i <= o.stars }">★</span>
            </div>

            <div v-if="o.variant !== 'A'" class="topPrice">CENA</div>
            <div class="title">TYTUŁ</div>

            <div v-if="o.variant === 'A'" class="bottom">
              <span class="user">USER</span>
              <span class="price">CENA</span>
            </div>
          </div>
        </button>
      </div>
    </Container>
  </section>
</template>

<style scoped>

</style>
