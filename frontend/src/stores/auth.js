import { reactive, computed } from "vue";
import { ROLES, ALL_ROLES } from "../auth/roles";

const state = reactive({
  user: null,
  loading: false,
});

function normalizeRole(payload) {
  const roleFromSingle =
    payload?.role || payload?.Role || payload?.userRole || payload?.UserRole;

  const rolesFromArray =
    payload?.roles ||
    payload?.Roles ||
    payload?.userRoles ||
    payload?.UserRoles;

  let role = roleFromSingle;

  if (!role && Array.isArray(rolesFromArray) && rolesFromArray.length) {
    if (rolesFromArray.includes("Admin")) role = "Admin";
    else if (rolesFromArray.includes("Freelancer")) role = "Freelancer";
    else if (rolesFromArray.includes("User")) role = "User";
    else role = rolesFromArray[0];
  }

  if (role === "Admin") return ROLES.ADMIN ?? "Admin";
  if (role === "Freelancer") return ROLES.FREELANCER ?? "Freelancer";
  if (role === "User") return ROLES.USER ?? "User";

  if (ALL_ROLES.includes(role)) return role;

  return ROLES.GUEST;
}

export function useAuth() {
  const role = computed(() => state.user?.role || ROLES.GUEST);
  const isLoggedIn = computed(() => role.value !== ROLES.GUEST);

  function hasRole(...roles) {
    return roles.includes(role.value);
  }

  async function initAuth() {
    state.loading = true;
    try {
      const res = await fetch("/api/user/getUser", {
        method: "GET",
        credentials: "include",
      });

      if (!res.ok) {
        state.user = null;
        return;
      }

      const data = await res.json();

      state.user = {
        username: data?.userName || data?.username || data?.login || "unknown",
        roles: data?.roles || data?.Roles || [],
        role: normalizeRole(data),
        raw: data,
      };
    } catch {
      state.user = null;
    } finally {
      state.loading = false;
    }
  }

  async function logout() {
    try {
      await fetch("/api/auth/logout", {
        method: "POST",
        credentials: "include",
      });
    } catch {
    } finally {
      state.user = null;
    }
  }

  function setMockRole(newRole) {
    if (!ALL_ROLES.includes(newRole)) return;
    state.user =
      newRole === ROLES.GUEST
        ? null
        : { id: "demo", email: "demo@demo.com", role: newRole };
  }

  return {
    state,
    role,
    isLoggedIn,
    hasRole,
    initAuth,
    logout,
    setMockRole,
  };
}
