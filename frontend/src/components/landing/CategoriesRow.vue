<script setup>
import Container from "../ui/Container.vue"
import { ref, onMounted, onBeforeUnmount } from "vue"

const topCats = ["KAT A", "KAT B", "KAT C", "KAT D", "KAT E"]
const allCats = ["KAT1", "KAT2", "KAT3", "KAT4", "KAT5", "ADVANCED..."]

const isOpen = ref(false)

function toggle() 
{
  isOpen.value = !isOpen.value
}
function close() 
{
  isOpen.value = false
}

//zamknięcie poza kategorią
function onDocClick(e) 
{
  const wrap = e.target.closest(".catWrap")
  if (!wrap) close()
}

onMounted(() => document.addEventListener("click", onDocClick))
onBeforeUnmount(() => document.removeEventListener("click", onDocClick))
</script>

<template>
  <section class="wrap">
    <Container>
      <div class="row">
        <div class="catWrap">
          <button class="tile primary" type="button" @click.stop="toggle">
            <div class="title">Kategorie</div>
            <div class="burger" aria-hidden="true">
              <span /><span /><span />
            </div>
          </button>

          <div v-if="isOpen" class="dropdown" @click.stop>
            <button
              v-for="c in allCats"
              :key="c"
              class="dropItem"
              type="button"
              @click="pick(c)"
            >
              {{ c }}
            </button>
          </div>
        </div>

        <button
          v-for="c in topCats"
          :key="c"
          class="tile"
          type="button"
          @click="pick(c)"
        >
          <div class="cat">{{ c }}</div>
        </button>
      </div>
    </Container>
  </section>
</template>

<style scoped>
.wrap{ padding: 10px 0 18px; }

.row
{
  display:grid;
  grid-template-columns: 240px repeat(5, 1fr);
  gap: 18px;
  border-top: 1px solid rgba(0,0,0,.12);
  border-bottom: 1px solid rgba(0,0,0,.12);
  padding: 14px 0;
  justify-items: center;
}

.tile
{
  width: 100%;
  max-width: 300px;
  border: 1px solid rgba(0,0,0,.12);
  background: #dcdcdc;
  border-radius: 8px;
  height: 86px;
  cursor: pointer;
  display:flex;
  align-items:center;
  justify-content:center;
}
.tile:hover{ background:#d3d3d3; }

.primary
{
  max-width: 240px;
  background:#9a9a9a;
  color:#fff;
  display:flex;
  justify-content:space-between;
  padding: 14px;
}
.title{ font-weight: 800; font-size: 18px; }

.burger
{
  display:flex;
  flex-direction:column;
  gap:6px;
  margin-top: 6px;
}

.burger span
{
  width: 46px;
  height: 4px;
  background:#fff;
  border-radius: 999px;
  opacity:.9;
}

.cat
{
  font-weight: 900;
  color: rgba(0,0,0,.55);
  letter-spacing: .6px;
  font-size: 18px;
}

.catWrap
{
  position: relative;
  width: 100%;
  max-width: 240px;
}

.dropdown
{
  position: absolute;
  left: 0;
  top: calc(86px + 10px);
  width: 100%;
  background: #e3e3e3;
  border: 1px solid rgba(0,0,0,.14);
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 10px 24px rgba(0,0,0,.12);
  z-index: 20;
}

.dropItem
{
  width: 100%;
  text-align: left;
  padding: 12px 14px;
  border: none;
  background: transparent;
  cursor: pointer;
  font-weight: 800;
  color: rgba(0,0,0,.55);
}

.dropItem:hover
{
  background: rgba(0,0,0,.06);
}

@media (max-width: 1100px)
{
  .row{ grid-template-columns: 240px repeat(3, 1fr); }
}

@media (max-width: 700px)
{
  .row{ grid-template-columns: 2fr 1fr; }
  .catWrap{ max-width: none; }
  .primary{ max-width: none; }
  .tile{ max-width: none; }
}
</style>
