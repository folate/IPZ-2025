<script setup>
import { useRouter } from "vue-router"
import { ErrorMessage, Field, Form } from "vee-validate"
import * as yup from "yup"

import LandingHeader from "../../components/landing/LandingHeader.vue"
import Container from "../../components/ui/Container.vue"

import { Card, CardContent } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Button } from "@/components/ui/button"
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"

const router = useRouter()

//opcje
const serviceTypes = [
  { value: "website", label: "Projektowanie stron (Website)" },
  { value: "logo", label: "Projektowanie Logo" },
  { value: "seo", label: "Optymalizacja SEO" },
  { value: "copywriting", label: "Copywriting" },
  { value: "video", label: "Montaż wideo" },
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
  title: yup.string().required("Wymagany tytuł usługi").min(3, "Tytuł musi mieć co najmniej 3 znaki"),
  description: yup.string().required("Wymagany opis").min(10, "Opis musi mieć co najmniej 10 znaków"),
  type: yup.string().required("Typ usługi jest wymagany"),
  budget: yup.number().transform((value, originalValue) => {
    if (originalValue === "" || originalValue === null || originalValue === undefined) return null
    const n = Number(String(originalValue).replace(",", "."))
    return Number.isNaN(n) ? NaN : n
  }).nullable().typeError("Budżet musi być liczbą").required("Budżet jest wymagany").moreThan(0, "Budżet musi wynosić więcej niż 0"),
  deadline: yup.date().typeError("Nieprawidłowa data").required("Termin jest wymagany").min(new Date(), "Termin musi być w przyszłości"),
  tags: yup.array().of(yup.string()).nullable(),
})

async function onSubmit(values)
{
  console.log("SUBMIT buyer ad:", values)
  alert("Opublikowano formularz (Mock)! Zobacz konsolę.")
  router.push("/buyer/profile")
}
</script>

<template>
  <div class="min-h-svh bg-zinc-50 dark:bg-zinc-950 pb-20">
    <LandingHeader />

    <Container>
      <div class="mt-8 flex flex-col gap-6 max-w-3xl mx-auto">
        
        <div>
          <h1 class="text-3xl font-extrabold text-zinc-900 dark:text-zinc-50">Kreator Ogłoszenia</h1>
          <p class="text-zinc-500 dark:text-zinc-400 mt-1">Opisz dokładnie czego potrzebujesz, aby otrzymać jak najlepsze oferty od freelancerów.</p>
        </div>

        <Card class="border-zinc-200 dark:border-zinc-800 shadow-xl shadow-teal-900/5">
          <CardContent class="p-6 md:p-8">
            <Form
              :validation-schema="schema"
              :initial-values="{
                title: '',
                description: '',
                type: '',
                budget: null,
                deadline: null,
                tags: [],
              }"
              @submit="onSubmit"
            >
              <div class="flex flex-col gap-6">
                
                <!-- Title -->
                <div class="flex flex-col gap-2">
                  <Label for="title" class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Tytuł zlecenia</Label>
                  <Field name="title" v-slot="{ field }">
                    <Input id="title" v-bind="field" placeholder="np. Nowoczesna strona internetowa dla firmy" class="h-11 rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus-visible:ring-teal-500" />
                  </Field>
                  <div class="h-4"><ErrorMessage name="title" class="text-xs text-red-500 font-medium block" /></div>
                </div>

                <!-- Description -->
                <div class="flex flex-col gap-2">
                  <Label for="description" class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Opis zadania</Label>
                  <Field name="description" v-slot="{ field }">
                    <Textarea id="description" v-bind="field" placeholder="Szczegóły zlecenia, wymagania, preferencje..." class="min-h-[150px] rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus-visible:ring-teal-500 resize-y" />
                  </Field>
                  <div class="h-4"><ErrorMessage name="description" class="text-xs text-red-500 font-medium block" /></div>
                </div>

                <div class="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-0">
                  
                  <!-- Type -->
                  <div class="flex flex-col gap-2">
                    <Label for="type" class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Kategoria</Label>
                    <Field name="type" v-slot="{ field, handleChange }">
                      <Select :model-value="field.value" @update:model-value="handleChange">
                        <SelectTrigger id="type" class="h-11 rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus:ring-teal-500">
                          <SelectValue placeholder="Wybierz kategorię" />
                        </SelectTrigger>
                        <SelectContent class="bg-white dark:bg-zinc-950 border-zinc-200 dark:border-zinc-800 rounded-xl">
                          <SelectGroup>
                            <SelectItem v-for="t in serviceTypes" :key="t.value" :value="t.value" class="focus:bg-zinc-100 dark:focus:bg-zinc-900 focus:text-teal-700 dark:focus:text-teal-400 cursor-pointer">
                              {{ t.label }}
                            </SelectItem>
                          </SelectGroup>
                        </SelectContent>
                      </Select>
                    </Field>
                    <div class="h-4"><ErrorMessage name="type" class="text-xs text-red-500 font-medium block" /></div>
                  </div>

                  <!-- Budget -->
                  <div class="flex flex-col gap-2">
                    <Label for="budget" class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Budżet (PLN)</Label>
                    <Field name="budget" v-slot="{ field }">
                      <Input id="budget" type="number" step="0.01" v-bind="field" placeholder="np. 500" class="h-11 rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus-visible:ring-teal-500" />
                    </Field>
                    <div class="h-4"><ErrorMessage name="budget" class="text-xs text-red-500 font-medium block" /></div>
                  </div>

                </div>

                <div class="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-0">
                  
                  <!-- Deadline -->
                  <div class="flex flex-col gap-2">
                    <Label for="deadline" class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Termin oddania</Label>
                    <Field name="deadline" v-slot="{ field }">
                      <Input id="deadline" type="date" v-bind="field" class="h-11 rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus-visible:ring-teal-500" />
                    </Field>
                    <div class="h-4"><ErrorMessage name="deadline" class="text-xs text-red-500 font-medium block" /></div>
                  </div>

                  <!-- AI Tags (Simplified for now standard multiselect would require more complex Shadcn combobox) -->
                  <div class="flex flex-col gap-2">
                    <Label for="tags" class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Targi (opcjonalne, przytrzymaj Ctrl aby zaznaczyć wiele)</Label>
                    <Field name="tags" v-slot="{ field, handleChange }">
                      <select id="tags" multiple @change="(e) => handleChange(Array.from(e.target.selectedOptions).map(o => o.value))" class="rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 focus-visible:ring-teal-500 h-[88px] text-sm text-zinc-900 dark:text-zinc-200 p-2">
                         <option v-for="t in aiTags" :key="t.value" :value="t.value" class="p-1 hover:bg-zinc-100 dark:hover:bg-zinc-800 cursor-pointer rounded">
                           {{ t.label }}
                         </option>
                      </select>
                    </Field>
                    <div class="h-4"><ErrorMessage name="tags" class="text-xs text-red-500 font-medium block" /></div>
                  </div>
                </div>

                <div class="pt-6 border-t border-zinc-100 dark:border-zinc-800/50 mt-2">
                  <Button type="submit" class="w-full h-12 rounded-xl bg-teal-600 hover:bg-teal-700 text-white shadow-md font-semibold text-base transition-colors">
                    Publikuj zlecenie
                  </Button>
                </div>

              </div>
            </Form>
          </CardContent>
        </Card>

      </div>
    </Container>
  </div>
</template>
