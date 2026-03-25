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
  const availableRoles = computed(() => state.user?.roles ?? []);

  function hasRole(...rolesToCheck) {
    return rolesToCheck.includes(role.value);
  }

  function canSwitchRole() {
    return (state.user?.roles?.includes("Seller") || state.user?.roles?.includes("Freelancer")) && state.user?.roles?.includes("Buyer");
  }

  function switchRole(newRole) {
    const isSellerCandidate = newRole === ROLES.SELLER && (availableRoles.value.includes("Seller") || availableRoles.value.includes("Freelancer"));
    const isBuyerCandidate = newRole === ROLES.BUYER && availableRoles.value.includes("Buyer");
    if (!state.user || !(isSellerCandidate || isBuyerCandidate)) return;
    state.user.role = newRole;
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

      const roles = Array.isArray(data?.roles) ? data.roles : [];
      const finalRole = mapRole(roles);

      state.user = {
        id: data?.id ?? null,
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
    canSwitchRole,
    switchRole,
    initAuth,
    setMockRole,
    logout,
  };
}
