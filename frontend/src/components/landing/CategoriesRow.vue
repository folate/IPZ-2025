<script setup>
import Container from "../ui/Container.vue";
import * as vue from "vue";

// const topCats = ["KAT A", "KAT B", "KAT C", "KAT D", "KAT E"];
// const allCats = ["KAT1", "KAT2", "KAT3", "KAT4", "KAT5", "ADVANCED..."];
const categoryList = vue.ref([]);
const TopcategoryList = vue.ref([]);
const isOpen = vue.ref(false);
const fetchTopCategories = async () => {
  try {
    const response = await fetch("/api/Category/top");
    if (response.ok) {
      const data = await response.json();
      TopcategoryList.value = data;
    }
  } catch (err) {
    console.error("Failed to fetch categories:", err);
  }
};
const fetchCategories = async () => {
  try {
    const response = await fetch("/api/Category");
    if (response.ok) {
      const data = await response.json();
      categoryList.value = data;
    }
  } catch (err) {
    console.error("Failed to fetch categories:", err);
  }
};
function useSortedCategories(listRef) {
  return vue.computed(() => {
    return [...listRef.value].sort((a, b) => a.name.localeCompare(b.name));
  });
}
function toggle() {
  isOpen.value = !isOpen.value;
}
function close() {
  isOpen.value = false;
}

//zamknięcie poza kategorią
function onDocClick(e) {
  const wrap = e.target.closest(".catWrap");
  if (!wrap) close();
}

vue.onMounted(() => {
  document.addEventListener("click", onDocClick);
  fetchTopCategories();
  fetchCategories();
});

vue.onBeforeUnmount(() => document.removeEventListener("click", onDocClick));

const sortedCategories = useSortedCategories(categoryList);
const sortedTopCategories = useSortedCategories(TopcategoryList);
</script>

<template>
  <section class="wrap">
    <Container>
      <div class="row">
        <div class="catWrap">
          <button class="tile primary" type="button" @click.stop="toggle">
            <div class="title">Categories</div>
            <div class="burger" aria-hidden="true">
              <span /><span /><span />
            </div>
          </button>

          <div v-if="isOpen" class="dropdown" @click.stop>
            <button
              v-for="category in sortedCategories"
              :key="category.name"
              :value="category.name"
              class="dropItem"
              @click="pick(category.name)"
            >
              {{ category.name }}
            </button>
          </div>
        </div>

        <button
          v-for="category in sortedTopCategories"
          :key="category.name"
          :value="category.name"
          class="tile"
          @click="pick(category.name)"
        >
          <div class="cat">{{ category.name }}</div>
        </button>
      </div>
    </Container>
  </section>
</template>
