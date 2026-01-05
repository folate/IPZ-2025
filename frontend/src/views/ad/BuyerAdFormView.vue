<script setup>
import { ref } from "vue"
import LandingHeader from "../../components/landing/LandingHeader.vue"
import Container from "../../components/ui/Container.vue"

// model formularza
const form = ref({
  title: "",
  description: "",
  type: null,
  budget: null,
  deadline: null,
  tags: [],
})

// opcje (mock)
const serviceTypes = [
  { value: "website", label: "Website design" },
  { value: "logo", label: "Logo design" },
  { value: "seo", label: "SEO" },
  { value: "copywriting", label: "Copywriting" },
  { value: "video", label: "Video editing" },
]

const aiTags = [
  { value: "ai", label: "AI" },
  { value: "frontend", label: "Frontend" },
  { value: "backend", label: "Backend" },
  { value: "design", label: "Design" },
  { value: "marketing", label: "Marketing" },
  { value: "automation", label: "Automation" },
]

function onSubmit()
{
  // TODO: podepniesz endpoint create buyer ad
  console.log("SUBMIT buyer ad:", JSON.parse(JSON.stringify(form.value)))
  alert("Saved (mock). Check console.")
}
</script>

<template>
  <div class="page">
    <LandingHeader />

    <Container>
      <div class="wrap">
        <div class="topRow">
          <h1 class="title">Buyer ad form</h1>
        </div>

        <Vueform v-model="form" @submit="onSubmit">
          <TextElement
            name="title"
            label="Service title"
            placeholder="e.g. Website design"
            rules="required|min:3"
          />

          <TextareaElement
            name="description"
            label="Service description"
            placeholder="Describe what you need..."
            rules="required|min:10"
          />

          <SelectElement
            name="type"
            label="Service type"
            :items="serviceTypes"
            placeholder="Choose type"
            rules="required"
            :search="true"
          />

          <TextElement
            name="budget"
            label="Budget"
            placeholder="e.g. 500"
            inputmode="decimal"
            rules="required"
          />

          <DateElement
            name="deadline"
            label="Deadline"
            placeholder="Choose deadline"
            rules="required"
          />

          <MultiselectElement
            name="tags"
            label="AI tagging"
            :items="aiTags"
            placeholder="Select tags"
            :search="true"
            mode="tags"
          />

          <ButtonElement name="submit" submits>
            Publish ad
          </ButtonElement>
        </Vueform>

        <!-- DEV: podgląd modelu -->
        <pre class="debug">{{ form }}</pre>
      </div>
    </Container>
  </div>
</template>

<style scoped>
.page{
  background: #f0f0f0;
  min-height: 100vh;
}

.wrap{
  padding: 22px 0 60px;
}

.topRow{
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 16px;
}

.title{
  font-size: 34px;
  font-weight: 900;
  margin: 0;
  color: rgba(0,0,0,.75);
}

.debug{
  margin-top: 18px;
  background: #ffffff;
  border: 2px solid rgba(0,0,0,.12);
  border-radius: 10px;
  padding: 12px;
  overflow: auto;
}
</style>
