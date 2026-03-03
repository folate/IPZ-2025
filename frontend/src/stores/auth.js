import { reactive, computed } from "vue";
import { ROLES, ALL_ROLES } from "../auth/roles";

const state = reactive({
  user: null,
  loading: false,
});

function mapRole(roles) {
  if (!Array.isArray(roles)) return ROLES.GUEST;

  if (roles.includes("Admin")) return ROLES.ADMIN;

  if (roles.includes("Seller") || roles.includes("Freelancer"))
    return ROLES.SELLER;

  if (roles.includes("Buyer")) return ROLES.BUYER;

  return ROLES.BUYER;
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
        role: ALL_ROLES.includes(finalRole) ? finalRole : ROLES.BUYER,
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

  async function logout() {
    try {
      await fetch("/api/Auth/logout", {
        method: "POST",
        credentials: "include",
      });
    } catch (e) {
      console.warn("Logout request failed:", e);
    } finally {
      state.user = null;
    }
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
