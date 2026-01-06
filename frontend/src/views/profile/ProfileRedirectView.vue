<script setup>
import { onMounted } from "vue"
import { useRouter } from "vue-router"
import { useAuth } from "@/stores/auth"
import { ROLES } from "@/auth/roles"

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
  <div style="padding: 40px">Redirecting...</div>
</template>