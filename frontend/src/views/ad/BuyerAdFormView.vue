<script setup>
import { ref } from "vue"
import * as yup from "yup"
import LandingHeader from "../../components/landing/LandingHeader.vue"
import Container from "../../components/ui/Container.vue"

const form$ = ref(null)

const form = ref({
  title: "",
  description: "",
  type: null,
  budget: null,
  deadline: null,
  tags: [],
})

//opcje
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

const schema = yup.object({
  title: yup
    .string()
    .required("Service title is required")
    .min(3, "Service title must be at least 3 characters"),
  description: yup
    .string()
    .required("Service description is required")
    .min(10, "Service description must be at least 10 characters"),
  type: yup
    .mixed()
    .nullable()
    .required("Service type is required"),
  budget: yup
    .number()
    .transform((value, originalValue) => {
      if (originalValue === "" || originalValue === null || originalValue === undefined) return null
      const n = Number(String(originalValue).replace(",", "."))
      return Number.isNaN(n) ? NaN : n
    })
    .nullable()
    .typeError("Budget must be a number")
    .required("Budget is required")
    .moreThan(0, "Budget must be greater than 0"),
  deadline: yup
    .date()
    .typeError("Deadline is required")
    .required("Deadline is required")
    .min(new Date(), "Deadline must be in the future"),
  tags: yup
    .array()
    .of(yup.mixed())
    .nullable(),
})

async function onSubmit()
{
  const f = form$.value

  if (f)
  {
    f.clearMessages()

    const fields = ["title", "description", "type", "budget", "deadline", "tags"]
    fields.forEach((name) => {
      const el = f.el$(name)
      if (el) el.clearMessages()
    })
  }

  try
  {
    await schema.validate(form.value, { abortEarly: false })

    console.log("SUBMIT buyer ad:", JSON.parse(JSON.stringify(form.value)))
    alert("Saved (mock). Check console.")
  }
  catch (err)
  {
    if (!form$.value) return

    const f2 = form$.value
    const inner = err?.inner || []

    if (!inner.length)
    {
      f2.messageBag.append(err?.message || "Validation error")
      return
    }

    const used = new Set()

    inner.forEach((e) => {
      if (!e?.path) return
      if (used.has(e.path)) return
      used.add(e.path)

      const el = f2.el$(e.path)
      if (el) el.messageBag.append(e.message)
    })
  }
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

        <Vueform ref="form$" v-model="form" @submit="onSubmit">
          <TextElement
            name="title"
            label="Service title"
            placeholder="e.g. Website design"
          />

          <TextareaElement
            name="description"
            label="Service description"
            placeholder="Describe what you need..."
          />

          <SelectElement
            name="type"
            label="Service type"
            :items="serviceTypes"
            placeholder="Choose type"
            :search="true"
          />

          <TextElement
            name="budget"
            label="Budget"
            placeholder="e.g. 500"
            inputmode="decimal"
          />

          <DateElement
            name="deadline"
            label="Deadline"
            placeholder="Choose deadline"
          />

          <MultiselectElement
            name="tags"
            label="AI tagging"
            :items="aiTags"
            placeholder="Select tags"
            :search="true"
            mode="tags"
            :close-on-select="false"
            :hide-selected="false"
          />

          <ButtonElement name="submit" submits>
            Publish ad
          </ButtonElement>
        </Vueform>

        <pre class="debug">{{ form }}</pre>
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

.wrap
{
  padding: 22px 0 60px;
}

.topRow
{
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 16px;
}

.title
{
  font-size: 34px;
  font-weight: 900;
  margin: 0;
  color: rgba(0,0,0,.75);
}

.debug
{
  margin-top: 18px;
  background: #ffffff;
  border: 2px solid rgba(0,0,0,.12);
  border-radius: 10px;
  padding: 12px;
  overflow: auto;
}
</style>
