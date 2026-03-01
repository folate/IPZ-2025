import { createRouter, createWebHistory } from "vue-router";
import LandingView from "../views/LandingView.vue";
import OfferDetailsCard from "../views/OfferDetails/OfferDetailsCard.vue";
import Settings from "@/components/Settings.vue";
import SettingsDeafultOrderMethods from "@/components/SettingsComponents/SettingsDeafultOrderMethods.vue";
import SettingsMailNotifs from "@/components/SettingsComponents/SettingsMailNotifs.vue";
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
      component: OfferDetailsCard,
      props: true,
    },
    {
      path: "/request/:id",
      name: "BuyerAdDetails",
      component: () => import("../views/OfferDetails/BuyerAdDetailsView.vue"),
    },
    //liked
    {
      path: "/liked",
      name: "Liked",
      component: () => import("../views/Liked/Liked.vue"),
    },
    //settings
    {
      path: "/buyer/profile/settings",
      component: Settings,
      children: [
        {
          path: "mail",
          component: SettingsMailNotifs,
        },
        {
          path: "methods",
          component: SettingsDeafultOrderMethods,
        },
      ],
    },
  ],
});

export default router;
