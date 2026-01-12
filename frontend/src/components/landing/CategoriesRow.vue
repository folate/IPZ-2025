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

