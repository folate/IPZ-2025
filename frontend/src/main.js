import { createApp } from "vue"
import App from "./App.vue"
import router from "./router"
import "./assets/main.css"

import Vueform from "@vueform/vueform"
import vueformConfig from "./vueform.config"
import "@vueform/vueform/themes/vueform/css/index.min.css"

createApp(App)
  .use(router)
  .use(Vueform, vueformConfig)
  .mount("#app")
