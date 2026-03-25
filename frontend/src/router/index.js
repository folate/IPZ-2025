import { createRouter, createWebHistory } from "vue-router";
import LandingView from "../views/LandingView.vue";
import OfferDetailsCard from "../views/OfferDetails/OfferDetailsCard.vue";
import Settings from "@/components/Settings.vue";
import SettingsDeafultOrderMethods from "@/components/SettingsComponents/SettingsDeafultOrderMethods.vue";
import SettingsMailNotifs from "@/components/SettingsComponents/SettingsMailNotifs.vue";
import PaymentView from "../views/payment/PaymentView.vue";
import ThanksView from "../views/payment/ThanksView.vue";
import OrderRevisionView from "../views/OrderRevisionView.vue";
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "landing",
      component: LandingView,
      meta: { title: "Home" },
    },
    {
      path: "/login",
      name: "login",
      component: () => import("../views/LoginView.vue"),
      meta: { title: "Login" },
    },
    {
      path: "/register",
      name: "register",
      component: () => import("../views/RegisterView.vue"),
      meta: { title: "Register" },
    },

    {
      path: "/buyer/ad",
      name: "buyer-ad-form",
      component: () => import("../views/ad/BuyerAdFormView.vue"),
      meta: { title: "Add Request", requiresAuth: true },
    },

    // (jeśli masz seller/buyer profile to zostaw swoje istniejące)
    {
      path: "/seller/profile/:id?",
      name: "seller-profile",
      component: () => import("../views/profile/SellerProfileView.vue"),
      meta: { title: "Seller Profile", requiresAuth: true },
    },
    {
      path: "/seller/onboarding",
      name: "seller-onboarding",
      component: () => import("../views/profile/SellerOnboardingView.vue"),
      meta: { title: "Become a Seller", requiresAuth: true },
    },
    {
      path: "/buyer/profile",
      name: "buyer-profile",
      component: () => import("../views/profile/BuyerProfileView.vue"),
      meta: { title: "Buyer Profile", requiresAuth: true },
    },
    //oferty
    {
      path: "/offer/:id",
      name: "OfferDetails",
      component: OfferDetailsCard,
      props: true,
      meta: { title: "Offer Details", requiresAuth: true },
    },
    {
      path: "/request/:id",
      name: "BuyerAdDetails",
      component: () => import("../views/OfferDetails/BuyerAdDetailsView.vue"),
      meta: { title: "Request Details", requiresAuth: true },
    },
    {
      path: "/request/:id/review",
      name: "BuyerAdReview",
      component: () => import("../views/OfferDetails/BuyerAdReviewView.vue"),
      meta: { title: "Review Offers", requiresAuth: true },
    },
    {
      path: "/payment",
      name: "payment",
      component: PaymentView,
      meta: { title: "Payment", requiresAuth: true },
    },
    {
      path: "/thanks",
      name: "thanks",
      component: ThanksView,
      meta: { title: "Thank You", requiresAuth: true },
    },
    {
      path: "/order/:id/revision",
      name: "OrderRevision",
      component: OrderRevisionView,
      meta: { title: "Order Revision", requiresAuth: true },
    },
    //liked
    {
      path: "/liked",
      name: "Liked",
      component: () => import("../views/Liked/Liked.vue"),
      meta: { title: "Liked Offers", requiresAuth: true },
    },
    //settings
    {
      path: "/buyer/profile/settings",
      component: Settings,
      meta: { title: "Settings", requiresAuth: true },
      children: [
        {
          path: "mail",
          component: SettingsMailNotifs,
          meta: { title: "Email Settings", requiresAuth: true },
        },
        {
          path: "methods",
          component: SettingsDeafultOrderMethods,
          meta: { title: "Payment Methods", requiresAuth: true },
        },
      ],
    },
    //search
    {
      path: "/search",
      name: "search",
      component: () => import("@/views/search/SearchView.vue"),
      meta: { title: "Search Results" },
    },
    {
      path: "/chat/:conversationId?",
      name: "chat",
      component: () => import("@/views/ChatView.vue"),
      meta: { title: "Messages", requiresAuth: true },
    },
  ],
});

router.beforeEach(async (to, from, next) => {
  const { isLoggedIn, state, initAuth } = (await import("@/stores/auth")).useAuth();
  
  // Inicjalizuj autoryzację, jeśli jeszcze nie jest (np. po odświeżeniu strony)
  if (!isLoggedIn.value && !state.user) {
    await initAuth();
  }

  const isGuestRoute = to.name === "login" || to.name === "register";

  if (to.meta.requiresAuth && !isLoggedIn.value) {
    next({ name: "login", query: { redirect: to.fullPath } });
  } else if (isGuestRoute && isLoggedIn.value) {
    next("/");
  } else {
    next();
  }
});

router.afterEach((to) => {
  const pageTitle = to.meta.title || "Marketplace";
  document.title = `Marketplace - ${pageTitle}`;
});

export default router;
