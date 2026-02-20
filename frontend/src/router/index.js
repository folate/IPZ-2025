import { createRouter, createWebHistory } from "vue-router";
import LandingView from "../views/LandingView.vue";
import OfferDetailsCard from "../views/OfferDetails/OfferDetailsCard.vue";
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "landing",
      component: LandingView,
    },
    {
      path: "/about",
      name: "about",
      component: () => import("../views/AboutView.vue"),
    },

    // Buyer ad form (vueform)
    {
      path: "/buyer/ad",
      name: "buyer-ad-form",
      component: () => import("../views/ad/BuyerAdFormView.vue"),
    },

    // (jeśli masz seller/buyer profile to zostaw swoje istniejące)
    {
      path: "/seller/profile",
      name: "seller-profile",
      component: () => import("../views/profile/SellerProfileView.vue"),
    },
    {
      path: "/buyer/profile",
      name: "buyer-profile",
      component: () => import("../views/profile/BuyerProfileView.vue"),
    },
    //oferty
    {
      path: "/offer/:id",
      name: "OfferDetails",
      component: () => import("../views/OfferDetails/OfferDetailsCard.vue"),
      props: true,
    },
    {
      path: "/liked",
      name: "Liked",
      component: () => import("../views/Liked/Liked.vue"),
    },
  ],
});

export default router;
