<script setup>
import { ref } from "vue"
import { useRouter } from "vue-router"
import LandingHeader from "../../components/landing/LandingHeader.vue"
import Container from "../../components/ui/Container.vue"

const router = useRouter()

// zakładki jak w sketchu
const tab = ref("offers") // offers | reviews | description

const tabs = [
  { key: "offers", label: "offers" },
  { key: "reviews", label: "reviews" },
  { key: "description", label: "description" },
]

// mocki – później pod backend
const seller = {
  name: "NAME",
  rating: 3,
  description: ["Lorem ipsum", "Lorem ipsum", "", "Lorem ipsum"],
}

const reviewStats = {
  total: 123,
  avg: 4.2,
  stars: [
    { stars: 1, count: 12 },
    { stars: 2, count: 9 },
    { stars: 3, count: 20 },
    { stars: 4, count: 35 },
    { stars: 5, count: 47 },
  ],
}

const reviews = [
  { id: 1, user: "username", stars: 3, text: "Lorem ipsum" },
  { id: 2, user: "username", stars: 3, text: "Lorem ipsum" },
  { id: 3, user: "username", stars: 3, text: "Lorem ipsum" },
]

const leftMenu = ["all", "cat 1", "cat 2", "cat 3", "orders"]

const activeLeft = ref("all")

//mock Orders
const orders = [
  { id: 101, user: "username1", title: "Order title 1" },
  { id: 102, user: "username2", title: "Order title 2" },
]

function onReport()
{
  //Report
  alert("report")
}

function goBuyerProfile()
{
  router.push("/buyer/profile")
}

function onOrderClick(order)
{
  goBuyerProfile()
}

function onServiceClick()
{
  goBuyerProfile()
}

const isFavourite = ref(false)

function onToggleFavourite()
{
  isFavourite.value = !isFavourite.value
  console.log("toggle favourite seller:", isFavourite.value)
}

function onChatClick()
{
  console.log("chat click")
}

const searchQuery = ref("")
</script>

<template>
  <div class="page">
    <LandingHeader />

    <div class="banner">
      <div class="bannerText">BANNER</div>
    </div>

    <Container>
      <div class="topRow">
        <div class="sellerMeta">
          <div class="avatarBox"></div>

          <div class="sellerText">
            <div class="sellerName">{{ seller.name }}</div>
            <div class="sellerStars" aria-label="rating">
              <span v-for="i in 5" :key="i" :class="{ on: i <= seller.rating }">★</span>
            </div>
          </div>
        </div>

        <div class="topRight">
          <div class="searchMini">
            <input
              v-model="searchQuery"
              class="searchInput"
              type="text"
              placeholder=""
            />
            <span class="searchLens"></span>
          </div>

          <button class="roundIconBtn" type="button" @click="onToggleFavourite">
            <img
              class="roundIconImg"
              src="/icons/favourites_icon.png"
              alt="favourites"
              :class="{ favOn: isFavourite }"
            />
          </button>

          <button class="roundIconBtn" type="button" @click="onChatClick">
            <img class="roundIconImg" src="/icons/chat.png" alt="chat" />
          </button>
        </div>
      </div>

      <div class="tabs">
        <button
          v-for="t in tabs"
          :key="t.key"
          class="tab"
          type="button"
          :class="{ active: tab === t.key }"
          @click="tab = t.key"
        >
          {{ t.label }}
        </button>
      </div>

      <div v-if="tab === 'description'" class="desc">
        <div v-for="(line, idx) in seller.description" :key="idx" class="descLine">
          {{ line }}
        </div>
      </div>

      <div v-else-if="tab === 'offers'" class="offersLayout">
        <aside class="leftMenu">
          <button
            v-for="m in leftMenu"
            :key="m"
            class="leftItem"
            type="button"
            :class="{ active: activeLeft === m }"
            @click="activeLeft = m"
          >
            {{ m }}
          </button>

          <!-- report w prostokącie -->
          <button class="reportBtn" type="button" @click="onReport">
            report
          </button>
        </aside>

        <section class="offersRight">

          <div v-if="activeLeft === 'orders'" class="ordersWrap">
            <div class="ordersTitle">Orders</div>

            <button
              v-for="o in orders"
              :key="o.id"
              class="orderCard"
              type="button"
              @click="onOrderClick(o)"
            >
              <div class="orderBox"></div>

              <div class="orderText">
                <div class="orderUser">{{ o.user }}</div>
                <div class="orderName">{{ o.title }}</div>
              </div>
            </button>
          </div>

          <template v-else>
            <div class="topOffersRow">
              <div class="topOffersTitle">Top offers</div>

              <div class="miniRow">
                <div v-for="i in 5" :key="i" class="miniCol">
                  <button class="miniCard" type="button" @click="onServiceClick"></button>
                  <div class="miniLabel">title</div>
                </div>
              </div>
            </div>

            <div class="allTitle">All</div>

            <div class="bigGrid">
              <div v-for="i in 5" :key="i" class="bigCol">
                <button class="bigCard" type="button" @click="onServiceClick"></button>
                <div class="bigLabel">title</div>
              </div>
            </div>
          </template>
        </section>
      </div>

      <div v-else class="reviewsLayout">
        <section class="reviewsLeft">
          <div v-for="r in reviews" :key="r.id" class="reviewRow">
            <div class="reviewAvatar"></div>
            <div class="reviewContent">
              <div class="reviewStars">
                <span v-for="i in 5" :key="i" :class="{ on: i <= r.stars }">★</span>
              </div>
              <div class="reviewText">{{ r.text }}</div>
              <div class="reviewUser">{{ r.user }}</div>
            </div>
          </div>
        </section>

        <aside class="reviewsStats">
          <div class="statTitle">num of reviews</div>
          <div class="statVal">{{ reviewStats.total }}</div>

          <div class="statTitle">average</div>
          <div class="statVal">{{ reviewStats.avg }}</div>

          <div class="starsBreakdown">
            <div v-for="s in reviewStats.stars" :key="s.stars" class="starLine">
              <span class="starIcons">
                <span v-for="i in s.stars" :key="i">★</span>
              </span>
              <span class="starCount">{{ s.stars }}</span>
            </div>
          </div>
        </aside>
      </div>
    </Container>
  </div>
</template>

<style scoped>
.page
{
  background: #f0f0f0;
  min-height: 100vh;
}

.banner
{
  margin: 0 auto;
  max-width: 1200px;
  height: 200px;
  background: #4d4d4d;
  border-radius: 8px;
  margin-top: 18px;
  position: relative;
}
.bannerText
{
  position: absolute;
  left: 40px;
  top: 35px;
  font-size: 120px;
  font-weight: 900;
  color: rgba(255,255,255,.25);
  letter-spacing: 4px;
}

.topRow
{
  margin-top: 18px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}

.sellerMeta
{
  display: flex;
  align-items: center;
  gap: 14px;
}
.avatarBox
{
  width: 70px;
  height: 70px;
  border-radius: 6px;
  background: #9a9a9a;
}
.sellerName
{
  font-size: 34px;
  font-weight: 900;
  color: rgba(0,0,0,.45);
}
.sellerStars
{
  margin-top: 4px;
  font-size: 18px;
  letter-spacing: 6px;
}
.sellerStars span{ color: rgba(0,0,0,.20); }
.sellerStars span.on{ color: rgba(0,0,0,.35); }

.topRight
{
  display: flex;
  align-items: center;
  gap: 14px;
}

.searchMini
{
  width: 220px;
  height: 44px;
  border-radius: 6px;
  background: #f2f2f2;
  border: 2px solid rgba(0,0,0,.25);
  position: relative;
  overflow: hidden;
}
.searchInput
{
  width: 100%;
  height: 100%;
  border: none;
  outline: none;
  background: transparent;
  padding: 0 44px 0 12px;
  font-size: 16px;
  font-weight: 700;
  color: rgba(0,0,0,.55);
}
.searchLens
{
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  width: 18px;
  height: 18px;
  border-radius: 999px;
  border: 3px solid rgba(0,0,0,.35);
  pointer-events: none;
}
.searchLens:after
{
  content: "";
  position: absolute;
  width: 10px;
  height: 3px;
  background: rgba(0,0,0,.35);
  right: -8px;
  bottom: -2px;
  transform: rotate(45deg);
  border-radius: 3px;
}
.roundIconBtn
{
  width: 44px;
  height: 44px;
  border-radius: 999px;
  background: #d7d7d7;
  display: grid;
  place-items: center;
  border: none;
  cursor: pointer;
  padding: 0;
}
.roundIconBtn:hover
{
  filter: brightness(.97);
}
.roundIconImg
{
  width: 30px;
  height: 30px;
  object-fit: contain;
}
.favOn
{
  filter: contrast(1.2);
}

.tabs
{
  margin-top: 16px;
  display: flex;
  gap: 18px;
  align-items: center;
}
.tab
{
  border: none;
  background: transparent;
  padding: 0;
  cursor: pointer;
  font-size: 26px;
  font-weight: 800;
  color: rgba(0,0,0,.35);
}
.tab.active
{
  color: rgba(0,0,0,.55);
  text-decoration: underline;
}

.desc
{
  margin-top: 18px;
  padding-bottom: 40px;
}
.descLine
{
  font-size: 44px;
  font-weight: 700;
  color: rgba(0,0,0,.22);
  line-height: 1.1;
}

.offersLayout
{
  margin-top: 18px;
  display: grid;
  grid-template-columns: 170px 1fr;
  gap: 18px;
  padding-bottom: 40px;
}
.leftMenu
{
  border-right: 3px solid rgba(0,0,0,.20);
  padding-right: 14px;
}
.leftItem
{
  display: block;
  width: 100%;
  text-align: left;
  border: none;
  background: transparent;
  cursor: pointer;
  font-size: 22px;
  font-weight: 800;
  color: rgba(0,0,0,.35);
  padding: 6px 0;
}
.leftItem.active
{
  color: rgba(0,0,0,.55);
  text-decoration: underline;
}

.reportBtn
{
  margin-top: 14px;
  width: 140px;
  height: 44px;

  border-radius: 6px;
  border: 2px solid rgba(0,0,0,.35);
  background: transparent;

  font-size: 22px;
  font-weight: 900;
  color: rgba(0,0,0,.45);

  cursor: pointer;
  text-align: center;
}

.reportBtn:hover
{
  background: rgba(0,0,0,.05);
}

.topOffersRow
{
  display: grid;
  grid-template-columns: 1fr;
  gap: 10px;
}
.topOffersTitle
{
  font-size: 30px;
  font-weight: 900;
  color: rgba(0,0,0,.45);
}

.miniRow
{
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 16px;
}
.miniCol
{
  display: grid;
  grid-template-columns: 1fr;
  gap: 10px;
}
.miniCard
{
  height: 70px;
  border-radius: 6px;
  border: 3px solid rgba(0,0,0,.25);
  background: #f5f5f5;
  cursor: pointer;
  padding: 0;
}
.miniCard:hover
{
  background: #eeeeee;
}
.miniLabel
{
  font-size: 22px;
  font-weight: 900;
  color: rgba(0,0,0,.35);
}

.allTitle
{
  margin-top: 8px;
  font-size: 30px;
  font-weight: 900;
  color: rgba(0,0,0,.45);
}

.bigGrid
{
  margin-top: 10px;
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 16px;
}
.bigCol
{
  display: grid;
  grid-template-columns: 1fr;
  gap: 10px;
}
.bigCard
{
  height: 90px;
  border-radius: 6px;
  border: 3px solid rgba(0,0,0,.25);
  background: #f5f5f5;
  cursor: pointer;
  padding: 0;
}
.bigCard:hover
{
  background: #eeeeee;
}
.bigLabel
{
  font-size: 22px;
  font-weight: 900;
  color: rgba(0,0,0,.35);
}

.ordersWrap
{
  display: grid;
  gap: 14px;
}

.ordersTitle
{
  font-size: 30px;
  font-weight: 900;
  color: rgba(0,0,0,.45);
}

.orderCard
{
  display: flex;
  align-items: center;
  gap: 14px;

  width: 100%;
  border-radius: 10px;
  border: 3px solid rgba(0,0,0,.18);
  background: #f5f5f5;

  padding: 14px;
  cursor: pointer;
  text-align: left;
}

.orderCard:hover
{
  background: #eeeeee;
}

.orderBox
{
  width: 70px;
  height: 50px;
  border-radius: 6px;
  background: #d7d7d7;
  border: 2px solid rgba(0,0,0,.18);
  flex: 0 0 auto;
}

.orderUser
{
  font-size: 22px;
  font-weight: 900;
  color: rgba(0,0,0,.45);
}

.orderName
{
  margin-top: 4px;
  font-size: 18px;
  font-weight: 800;
  color: rgba(0,0,0,.30);
}

.reviewsLayout
{
  margin-top: 18px;
  display: grid;
  grid-template-columns: 1fr 260px;
  gap: 24px;
  padding-bottom: 40px;
}
.reviewRow
{
  display: grid;
  grid-template-columns: 70px 1fr;
  gap: 16px;
  margin-bottom: 26px;
}
.reviewAvatar
{
  width: 70px;
  height: 70px;
  border-radius: 6px;
  border: 3px solid rgba(0,0,0,.25);
  background: #f5f5f5;
}
.reviewStars
{
  font-size: 20px;
  letter-spacing: 6px;
}
.reviewStars span{ color: rgba(0,0,0,.20); }
.reviewStars span.on{ color: rgba(0,0,0,.35); }
.reviewText
{
  font-size: 34px;
  font-weight: 800;
  color: rgba(0,0,0,.25);
  line-height: 1.1;
}
.reviewUser
{
  margin-top: 6px;
  font-size: 20px;
  font-weight: 800;
  color: rgba(0,0,0,.35);
}
.reviewsStats
{
  border-left: 3px solid rgba(0,0,0,.20);
  padding-left: 18px;
}
.statTitle
{
  font-size: 22px;
  font-weight: 900;
  color: rgba(0,0,0,.35);
  margin-top: 10px;
}
.statVal
{
  font-size: 32px;
  font-weight: 900;
  color: rgba(0,0,0,.45);
}
.starsBreakdown
{
  margin-top: 12px;
}
.starLine
{
  display: flex;
  align-items: center;
  gap: 12px;
  margin: 8px 0;
  color: rgba(0,0,0,.35);
  font-weight: 900;
}
.starIcons
{
  min-width: 90px;
  letter-spacing: 4px;
}
</style>
