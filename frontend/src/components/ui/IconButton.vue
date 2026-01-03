<script setup>
defineProps({
  label: { type: String, default: "" },
})
</script>

<template>
  <button class="btn" type="button" :aria-label="label" :title="label">
    <slot />
  </button>
</template>

<style scoped>
.btn
{
  width: 38px;
  height: 38px;
  border-radius: 10px;
  border: 1px solid rgba(0,0,0,.12);
  background: #f3f3f3;
  display: grid;
  place-items: center;
  cursor: pointer;
}
.btn:hover{ background:#ededed; }
</style>
