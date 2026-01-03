import { reactive, computed } from "vue"
import { ROLES, ALL_ROLES } from "../auth/roles"

const state = reactive({
  user: null,
  loading: false,
})

export function useAuth() 
{
  const role = computed(() => state.user?.role || ROLES.GUEST)
  const isLoggedIn = computed(() => role.value !== ROLES.GUEST)

  function hasRole(...roles) 
  {
    return roles.includes(role.value)
  }

  //pobranie usera z backendu
  async function initAuth() 
  {
    state.loading = true
    try {
      const res = await fetch("/api/auth/me", {
        method: "GET",
        credentials: "include",
      })

      if (res.ok) {
        state.user = await res.json()
      } else {
        state.user = null
      }
    } catch {
      state.user = null
    } finally {
      state.loading = false
    }
  }

  //TESTOWE
  function setMockRole(newRole) 
  {
    if (!ALL_ROLES.includes(newRole)) return
    state.user =
      newRole === ROLES.GUEST
        ? null
        : { id: "demo", email: "demo@demo.com", role: newRole }
  }
  function logout() 
  {
    state.user = null
  }

  return {
    state,
    role,
    isLoggedIn,
    hasRole,
    initAuth,
    setMockRole,
    logout,
  }
}
