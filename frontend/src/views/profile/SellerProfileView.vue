<script setup>
import { ref, computed, onMounted } from "vue";
import LandingHeader from "@/components/landing/LandingHeader.vue";
import OfferCard from "@/components/landing/OfferCard.vue";
import { useRouter } from "vue-router";

const router = useRouter();

const activeTab = ref("offers");
const loading = ref(false);
const error = ref("");

const me = ref(null);
const offers = ref([]);

const categories = ref([]);
const selectedCategory = ref("all");

const searchText = ref("");

const displayName = computed(() => me.value?.login ?? "NAME");

function offerOwnerLogin(o) {
  return (
    o?.freelancer ??
    o?.freelancerLogin ??
    o?.seller ??
    o?.sellerLogin ??
    o?.login ??
    null
  );
}

const filteredOffers = computed(() => {
  const q = searchText.value.trim().toLowerCase();
  const cat = selectedCategory.value;

  return (offers.value || []).filter((o) => {
    if (cat !== "all") {
      const offerCat = String(o?.category ?? "").trim();
      if (offerCat !== cat) return false;
    }

    if (!q) return true;

    const title = String(o?.title ?? "").toLowerCase();
    const desc = String(o?.description ?? "").toLowerCase();

    return title.includes(q) || desc.includes(q);
  });
});

async function load() {
  loading.value = true;
  error.value = "";
  me.value = null;
  offers.value = [];

  try {
    const resUser = await fetch("/api/User/getUser", {
      credentials: "include",
    });

    if (!resUser.ok) {
      error.value = "Nie jesteś zalogowany.";
      return;
    }

    const userData = await resUser.json();
    me.value = userData;

    const login = userData?.login;
    if (!login) {
      error.value = "Brak loginu w getUser.";
      return;
    }

    const resOffers = await fetch("/api/sellerad/all/200", {
      credentials: "include",
    });

    if (!resOffers.ok) {
      error.value = `Nie udało się pobrać ofert (${resOffers.status}).`;
      return;
    }

    const all = await resOffers.json();
    const list = Array.isArray(all) ? all : [];

    offers.value = list.filter((o) => offerOwnerLogin(o) === login);
  } catch (e) {
    error.value = e?.message ?? "Błąd podczas pobierania profilu.";
  } finally {
    loading.value = false;
  }
}

async function loadCategories() {
  try {
    const res = await fetch("/api/category", {
      credentials: "include",
    });

    if (!res.ok) return;

    const data = await res.json();
    categories.value = Array.isArray(data) ? data : [];
  } catch {
    categories.value = [];
  }
}

function selectCat(name) {
  selectedCategory.value = name;
}

onMounted(async () => {
  await Promise.all([load(), loadCategories()]);
});
</script>

<template>
  <div class="page">
    <LandingHeader />

    <div class="container">
      <div class="banner">BANNER</div>

      <div class="headerRow">
        <div class="avatar"></div>

        <div class="nameCol">
          <div class="name">{{ displayName }}</div>
          <div class="stars">★ ★ ★ ★ ★</div>
        </div>

        <div class="spacer"></div>

        <div class="searchBox">
          <input
            v-model="searchText"
            class="searchInput"
            placeholder="Szukaj w moich ofertach..."
          />

          <button class="searchBtn" type="button" aria-label="Szukaj">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
              <path
                d="M10.5 18a7.5 7.5 0 1 1 0-15 7.5 7.5 0 0 1 0 15Z"
                stroke="currentColor"
                stroke-width="2"
              />
              <path
                d="M16.5 16.5 21 21"
                stroke="currentColor"
                stroke-width="2"
              />
            </svg>
          </button>
        </div>

        <button class="circleBtn" type="button">
          <img class="circleIcon" src="/icons/favourites.png" alt="heart" />
        </button>

        <button class="circleBtn" type="button">
          <img class="circleIcon" src="/icons/chat.png" alt="chat" />
        </button>
      </div>

      <div class="tabs">
        <button
          class="tab"
          :class="{ active: activeTab === 'offers' }"
          @click="activeTab = 'offers'"
        >
          offers
        </button>
        <button
          class="tab"
          :class="{ active: activeTab === 'reviews' }"
          @click="activeTab = 'reviews'"
        >
          reviews
        </button>
        <button
          class="tab"
          :class="{ active: activeTab === 'description' }"
          @click="activeTab = 'description'"
        >
          description
        </button>
      </div>

      <div class="content">
        <p v-if="loading" class="info">Ładowanie...</p>
        <p v-else-if="error" class="error">{{ error }}</p>

        <template v-else>
          <div v-if="activeTab === 'offers'" class="offersTab">
            <div class="offersLayout">
              <aside class="cats">
                <button
                  type="button"
                  class="catBtn"
                  :class="{ active: selectedCategory === 'all' }"
                  @click="selectCat('all')"
                >
                  all
                </button>

                <button
                  v-for="c in categories"
                  :key="c.name"
                  type="button"
                  class="catBtn"
                  :class="{ active: selectedCategory === c.name }"
                  @click="selectCat(c.name)"
                >
                  {{ c.name }}
                </button>

                <button
                  type="button"
                  @click="router.push('/buyer/profile/settings')"
                >
                  Settings!
                </button>
              </aside>

              <section class="offersMain">
                <div class="sectionTitle">Top offers</div>

                <p v-if="filteredOffers.length === 0" class="info">
                  Brak ofert.
                </p>

                <div class="offersGrid" v-else>
                  <RouterLink
                    v-for="o in filteredOffers"
                    :key="o.id"
                    :to="`/offer/${o.id}`"
                    class="offerLink"
                  >
                    <OfferCard :offer="o" />
                  </RouterLink>
                </div>
              </section>
            </div>
          </div>

          <div v-else-if="activeTab === 'reviews'">
            <p class="info">Tu będą reviews.</p>
          </div>

          <div v-else>
            <p class="info">Tu będzie description.</p>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page {
  min-height: 100vh;
  background: #3f4a4d;
}

.container {
  max-width: 1500px;
  margin: 0 auto;
  padding: 22px 16px 60px;
}

.banner {
  height: 260px;
  border-radius: 14px;
  background: rgba(0, 0, 0, 0.35);
  border: 2px solid rgba(255, 255, 255, 0.25);
  display: flex;
  align-items: center;
  padding-left: 56px;
  font-size: 140px;
  font-weight: 800;
  color: rgba(255, 255, 255, 0.18);
}

.headerRow {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-top: 18px;
}

.avatar {
  width: 86px;
  height: 86px;
  border-radius: 10px;
  background: rgba(255, 255, 255, 0.35);
}

.nameCol {
  display: flex;
  flex-direction: column;
}

.name {
  font-size: 44px;
  font-weight: 800;
  color: rgba(255, 255, 255, 0.78);
}

.nameCol .stars {
  position: static !important;
  margin-top: 10px;
  color: rgba(255, 255, 255, 0.55);
  letter-spacing: 3px;
}

.spacer {
  flex: 1;
}

.searchBox {
  display: flex;
  align-items: center;
  gap: 10px;
}

.searchInput {
  width: 340px;
  height: 38px;
  border-radius: 9px;
  border: 2px solid rgba(0, 0, 0, 0.2);
  padding: 0 12px;
  outline: none;
}

.searchBtn {
  width: 38px;
  height: 38px;
  border-radius: 9px;
  border: 2px solid rgba(0, 0, 0, 0.2);
  background: rgba(255, 255, 255, 0.75);
  display: grid;
  place-items: center;
  cursor: pointer;
}

.searchBtn svg {
  color: rgba(0, 0, 0, 0.55);
}

.circleBtn {
  width: 46px;
  height: 46px;
  border-radius: 50%;
  border: none;
  background: rgba(255, 255, 255, 0.75);
  display: grid;
  place-items: center;
  cursor: pointer;
}

.circleIcon {
  width: 22px;
  height: 22px;
}

.tabs {
  display: flex;
  gap: 18px;
  margin-top: 18px;
}

.tab {
  background: transparent;
  border: none;
  font-size: 28px;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.55);
  cursor: pointer;
}

.tab.active {
  color: rgba(255, 255, 255, 0.9);
  text-decoration: underline;
}

.offersLayout {
  display: grid;
  grid-template-columns: 220px 1fr;
  gap: 18px;
  margin-top: 12px;
}

.cats {
  display: flex;
  flex-direction: column;
  gap: 14px;
  padding-top: 6px;
}

.catBtn {
  background: transparent;
  border: none;
  padding: 0;
  text-align: left;
  font-size: 22px;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.55);
  cursor: pointer;
}

.catBtn.active {
  color: rgba(255, 255, 255, 0.9);
  text-decoration: underline;
}

.sectionTitle {
  font-size: 40px;
  font-weight: 800;
  color: rgba(255, 255, 255, 0.78);
  margin: 8px 0 14px;
}

.offersGrid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 18px;
}

.offerLink {
  display: block;
  text-decoration: none;
  color: inherit;
}

.info {
  color: rgba(255, 255, 255, 0.7);
}

.error {
  color: #ffb4b4;
}
</style>
