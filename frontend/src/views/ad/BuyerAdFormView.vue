<script setup>
import { ref } from "vue"
import { useRouter } from "vue-router"
import { useAlert } from "@/stores/alert"
import { ErrorMessage, Field, Form } from "vee-validate"
import * as yup from "yup"

import Container from "../../components/ui/Container.vue"

import { Card, CardContent } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Button } from "@/components/ui/button"
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Calendar } from "@/components/ui/calendar"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/components/ui/command"
import { Badge } from "@/components/ui/badge"
import { Calendar as CalendarIcon, X, Search } from "lucide-vue-next"
import { cn } from "@/lib/utils"
import { DateFormatter, getLocalTimeZone, CalendarDate } from "@internationalized/date"

const df = new DateFormatter("pl-PL", { dateStyle: "long" })
const router = useRouter()
const { showAlert } = useAlert()

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

async function onSubmit(values) {
  try {
    const payload = {
      title: values.title,
      description: values.description,
      deadline: new Date(values.deadline).toISOString(),
      category: values.type,
      budget: Number(values.budget),
      // tags are ignored by the backend schema provided, but added here in case it's needed later or can be stripped safely
      // tags: values.tags || []
    }

    const res = await fetch("/api/BuyerAd/create", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
      credentials: "include"
    })

    if (!res.ok) {
      throw new Error(`Błąd serwera: ${res.status}`)
    }

    showAlert("Sukces", "Pomyślnie dodano zlecenie!")
    window.dispatchEvent(new Event("buyerad:created"));
    router.push("/buyer/profile")
  } catch (error) {
    console.error("Błąd podczas dodawania zlecenia:", error)
    showAlert("Błąd", "Wystąpił błąd podczas dodawania zlecenia.", "destructive")
  }
}
</script>

<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 pb-20">

    <Container>
      <div class="mt-8 flex flex-col gap-6 w-full">
        
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
                    <Field name="deadline" v-slot="{ value, handleChange }">
                      <Popover>
                        <PopoverTrigger as-child>
                          <Button
                            variant="outline"
                            :class="cn('h-11 justify-start text-left font-normal rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 border-zinc-200 dark:border-zinc-800 hover:bg-zinc-100 dark:hover:bg-zinc-900 focus-visible:ring-teal-500', !value && 'text-zinc-500')"
                          >
                            <CalendarIcon class="mr-2 h-4 w-4" />
                            <span>{{ value ? df.format(new Date(value)) : 'Wybierz datę z kalendarza' }}</span>
                          </Button>
                        </PopoverTrigger>
                        <PopoverContent class="w-auto p-0 z-[100] bg-white dark:bg-zinc-950 border-zinc-200 dark:border-zinc-800" align="start">
                          <Calendar
                            :model-value="value ? new CalendarDate(new Date(value).getFullYear(), new Date(value).getMonth() + 1, new Date(value).getDate()) : undefined"
                            @update:model-value="(v) => { if(v) { const d = v.toDate(getLocalTimeZone()); d.setHours(12); handleChange(d.toISOString()); } else { handleChange(null); } }"
                            initial-focus
                          />
                        </PopoverContent>
                      </Popover>
                    </Field>
                    <div class="h-4"><ErrorMessage name="deadline" class="text-xs text-red-500 font-medium block" /></div>
                  </div>

                  <!-- AI Tags -->
                  <div class="flex flex-col gap-3">
                    <Label for="tags" class="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Tagi (opcjonalne)</Label>
                    <Field name="tags" v-slot="{ value, handleChange }">
                      <div class="flex flex-col gap-3">
                        
                        <!-- Search Input (Triggers Dropdown via CSS focus-within) -->
                        <div class="relative w-full group">
                          <Command class="overflow-visible bg-transparent border-none p-0">
                            <div class="border border-zinc-200 dark:border-zinc-800 rounded-xl bg-zinc-50/50 dark:bg-zinc-900/50 overflow-hidden focus-within:ring-2 focus-within:ring-teal-500 transition-all flex items-center pr-3">
                              <CommandInput placeholder="Wpisz aby wyszukać tagi..." class="border-none focus:ring-0 shadow-none outline-none ring-0 w-full bg-transparent h-11" />
                            </div>
                            
                            <!-- The Dropdown (Absolute positioned) -->
                            <div class="absolute top-full left-0 w-full z-[100] mt-2 hidden group-focus-within:block" @mousedown.prevent>
                              <div class="bg-white dark:bg-zinc-950 border border-zinc-200 dark:border-zinc-800 rounded-xl shadow-xl overflow-hidden">
                                <CommandList class="max-h-60 overflow-y-auto w-full p-2">
                                  <CommandEmpty class="py-6 text-center text-sm text-zinc-500 dark:text-zinc-400">Nie znaleziono tagów.</CommandEmpty>
                                  <CommandGroup>
                                    <CommandItem
                                      v-for="t in aiTags.filter(t => !(value || []).includes(t.value))"
                                      :key="t.value"
                                      :value="t.label"
                                      @select="() => { handleChange([...(value || []), t.value]); }"
                                      class="cursor-pointer px-3 py-2 hover:bg-zinc-100 dark:hover:bg-zinc-800 rounded-lg font-medium"
                                    >
                                      {{ t.label }}
                                    </CommandItem>
                                  </CommandGroup>
                                </CommandList>
                              </div>
                            </div>
                          </Command>
                        </div>

                        <!-- Selected Tags Below -->
                        <div class="flex flex-wrap gap-2" v-if="value && value.length > 0">
                          <Badge 
                            v-for="tag in value" 
                            :key="tag" 
                            variant="secondary"
                            class="px-3 py-1.5 text-sm bg-teal-50 dark:bg-teal-900/30 text-teal-700 dark:text-teal-400 border-teal-200 dark:border-teal-800 rounded-lg flex items-center"
                          >
                            {{ aiTags.find(t => t.value === tag)?.label || tag }}
                            <button type="button" class="ml-2 hover:text-red-500 rounded-full focus:outline-none transition-colors" @click="handleChange((value || []).filter(v => v !== tag))">
                              <X class="h-3.5 w-3.5" />
                            </button>
                          </Badge>
                        </div>
                      </div>
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
