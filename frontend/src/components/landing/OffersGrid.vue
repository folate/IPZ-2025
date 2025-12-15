<script setup>
import Container from "../ui/Container.vue"

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
.wrap{ padding: 18px 0 46px; }

.grid
{
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 26px;
}

.card
{
  border: none;
  background: transparent;
  padding: 0;
  cursor: pointer;
  text-align: left;
}

.box
{
  position: relative;
  height: 170px;
  border-radius: 7px;
  background: #d0d0d0;
  border: 3px solid rgba(0,0,0,.18);
  overflow: hidden;
  width: 100%;
  max-width: 300px;
}

.stars
{
  position: absolute;
  top: 10px;
  right: 12px;
  font-size: 14px;
  letter-spacing: 2px;
}

.stars span{ color: rgba(0,0,0,.18); }
.stars span.on{ color: rgba(0,0,0,.35); }

.topPrice
{
  position: absolute;
  top: 10px;
  left: 12px;
  font-weight: 900;
  color: rgba(0,0,0,.55);
}

.title
{
  position: absolute;
  left: 12px;
  font-weight: 900;
  color: rgba(0,0,0,.55);
}

.bottom
{
  position: absolute;
  left: 12px;
  right: 12px;
  bottom: 12px;
  display: flex;
  justify-content: space-between;
  font-weight: 800;
  color: rgba(0,0,0,.45);
}


/* Różne warianty */
.v-A .title
{
  bottom: 44px;
}

.v-B .title
{
  bottom: 54px;
}

.v-C .title
{
  bottom: 64px;
}

@media (max-width: 1200px)
{
  .grid{ grid-template-columns: repeat(3, 1fr); }
}

@media (max-width: 850px)
{
  .grid{ grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 450px)
{
  .grid{ grid-template-columns: 1fr; }
}
</style>