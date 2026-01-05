import { createRouter, createWebHistory } from "vue-router"
import LandingView from "../views/LandingView.vue"

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "landing",
      component: LandingView,
    },

    {
      path: "/profile",
      name: "profile-redirect",
      component: () => import("../views/profile/ProfileRedirectView.vue"),
    },

    {
      path: "/buyer/profile",
      name: "buyer-profile",
      component: () => import("../views/profile/BuyerProfileView.vue"),
    },

    {
      path: "/seller/profile",
      name: "seller-profile",
      component: () => import("../views/profile/SellerProfileView.vue"),
    },

    {
      path: "/about",
      name: "about",
      component: () => import("../views/AboutView.vue"),
    },

    {
      path: "/buyer/ad",
      name: "buyer-ad-form",
      component: () => import("../views/ad/BuyerAdFormView.vue"),
    },
  ],
})

export default router