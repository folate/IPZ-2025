import { reactive, computed } from "vue";
import { ROLES, ALL_ROLES } from "../auth/roles";

const state = reactive({
  user: null,
  loading: false,
});

function mapRole(roles) {
  if (roles.includes("Admin")) return ROLES.ADMIN;
  if (roles.includes("Buyer")) return ROLES.BUYER;
  if (roles.includes("Seller")) return ROLES.SELLER;
  return ROLES.USER;
}

export function useAuth() {
  const role = computed(() => state.user?.role ?? ROLES.GUEST);
  const isLoggedIn = computed(() => role.value !== ROLES.GUEST);

  function hasRole(...rolesToCheck) {
    return rolesToCheck.includes(role.value);
  }

  async function initAuth() {
    state.loading = true;

    try {
      const res = await fetch("/api/User/getUser", {
        method: "GET",
        credentials: "include",
      });

      if (!res.ok) {
        state.user = null;
        return;
      }

      const data = await res.json();

      const roles = Array.isArray(data?.role) ? data.role : [];
      const finalRole = mapRole(roles);

      state.user = {
        login: data?.login ?? null,
        roles,
        role: ALL_ROLES.includes(finalRole) ? finalRole : ROLES.USER,
      };
    } catch (e) {
      state.user = null;
    } finally {
      state.loading = false;
    }
  }

  function setMockRole(newRole) {
    if (!ALL_ROLES.includes(newRole)) return;

    state.user =
      newRole === ROLES.GUEST
        ? null
        : { login: "demo", roles: [newRole], role: newRole };
  }

  function logout() {
    state.user = null;
  }

  return {
    state,
    role,
    isLoggedIn,
    hasRole,
    initAuth,
    setMockRole,
    logout,
  };
}
