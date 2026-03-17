<script setup>
import { onMounted } from "vue"
import { useRouter } from "vue-router"
import { useAuth } from "@/stores/auth"
import { ROLES } from "@/auth/roles"
import { Loader2 } from "lucide-vue-next"

const router = useRouter()
const { role } = useAuth()

onMounted(() => 
{
  switch (role.value) 
  {
    case ROLES.SELLER:
      router.replace("/seller/profile")
      break
    case ROLES.USER:
      router.replace("/buyer/profile")
      break
    default:
      router.replace("/") //guest
  }
})
</script>

<template>
  <div class="min-h-svh bg-zinc-50 dark:bg-zinc-950 flex flex-col items-center justify-center p-6 gap-4">
    <Loader2 class="h-10 w-10 text-teal-600 animate-spin" />
    <span class="text-xl font-semibold text-zinc-500 dark:text-zinc-400">Trwa przekierowanie...</span>
  </div>
</template>