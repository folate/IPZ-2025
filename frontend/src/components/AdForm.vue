<script setup>
import * as vue from "vue";
import * as yup from "yup";
import { Form, Field, ErrorMessage, FieldArray } from "vee-validate";
import { 
  Dialog, 
  DialogContent, 
  DialogHeader, 
  DialogTitle, 
  DialogDescription,
  DialogFooter
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { Plus, Trash2 } from "lucide-vue-next";

// We use a generic div for ScrollArea here to avoid installing more complex shadcn scroll-area element just for this modal.
const ScrollArea = "div"; 

const props = defineProps(["isOpen"]);
const emit = defineEmits(["close"]);
const categoryList = vue.ref([]);

const fetchCategories = async () => {
  try {
    const response = await fetch("/api/category");
    if (response.ok) {
      const data = await response.json();
      categoryList.value = data;
    }
  } catch (err) {
    console.error("Failed to fetch categories:", err);
  }
};

vue.onMounted(() => {
  fetchCategories();
});

const schema = yup.object({
  title: yup.string().required("Tytuł jest wymagany"),
  description: yup.string().required("Opis jest wymagany"),
  categories: yup.string().required("Proszę wybrać kategorię"),
  tiers: yup
    .array()
    .of(
      yup.object({
        name: yup.string().required("Nazwa pakietu jest wymagana"),
        price: yup
          .number()
          .typeError("Cena musi być liczbą")
          .required("Cena jest wymagana")
          .positive("Cena musi być dodatnia"),
        description: yup.string().required("Opis pakietu jest wymagany"),
      }),
    )
    .min(1, "Wymagany jest co najmniej jeden pakiet"),
});

const onSubmit = async (values, { setSubmitting }) => {
  try {
    const gigsPayload = values.tiers.map((tier) => ({
      TierName: tier.name,
      TierDescription: tier.description,
      Price: tier.price,
    }));

    const response = await fetch("/api/sellerad/create", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        Title: values.title,
        Description: values.description,
        Category: values.categories,
        Gigs: gigsPayload,
      }),
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error("Server Error:", response.status, errorText);
      return;
    }

    const result = await response.text();
    console.log("Success:", result);
    emit("close");
  } catch (err) {
    console.error("Submission Error:", err);
  } finally {
    setSubmitting(false);
  }
};
</script>

<template>
  <Dialog :open="isOpen" @update:open="(val) => !val && $emit('close')">
    <DialogContent class="sm:max-w-2xl bg-zinc-50 dark:bg-zinc-950 border-zinc-200 dark:border-zinc-800 shadow-2xl p-0 overflow-hidden rounded-2xl">
      <DialogHeader class="p-6 pb-4 border-b border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900">
        <DialogTitle class="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">Utwórz Ogłoszenie</DialogTitle>
        <DialogDescription class="text-zinc-500 dark:text-zinc-400">
          Wypełnij formularz poniżej, aby zaoferować swoje usługi.
        </DialogDescription>
      </DialogHeader>

      <ScrollArea class="max-h-[70vh] overflow-y-auto px-6 py-4">
        <Form
          :validation-schema="schema"
          :initial-values="{ tiers: [{ name: '', price: 0, description: '' }] }"
          @submit="onSubmit"
          v-slot="{ isSubmitting }"
        >
          <div class="space-y-6">
            <!-- Podstawowe Informacje -->
            <div class="space-y-4">
              <div class="grid gap-2">
                <Label class="text-zinc-700 dark:text-zinc-300">Tytuł Ogłoszenia</Label>
                <Field name="title" as="input" class="flex h-11 w-full rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 disabled:cursor-not-allowed disabled:opacity-50 transition-colors" placeholder="Np. Projektowanie logo" />
                <ErrorMessage name="title" class="text-xs text-red-500 font-medium" />
              </div>

              <div class="grid gap-2">
                <Label class="text-zinc-700 dark:text-zinc-300">Opis</Label>
                <Field name="description" as="textarea" class="flex min-h-[100px] w-full rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 disabled:cursor-not-allowed disabled:opacity-50 transition-colors" placeholder="Opisz dokładnie, co oferujesz..." />
                <ErrorMessage name="description" class="text-xs text-red-500 font-medium" />
              </div>

              <div class="grid gap-2">
                <Label class="text-zinc-700 dark:text-zinc-300">Kategoria</Label>
                <Field name="categories" as="select" class="flex h-11 w-full rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 disabled:cursor-not-allowed disabled:opacity-50 transition-colors appearance-none">
                  <option value="" disabled>Wybierz kategorię</option>
                  <option
                    v-for="category in categoryList"
                    :key="category.name"
                    :value="category.name"
                  >
                    {{ category.name }}
                  </option>
                </Field>
                <ErrorMessage name="categories" class="text-xs text-red-500 font-medium" />
              </div>
            </div>

            <div class="h-px w-full bg-zinc-200 dark:bg-zinc-800"></div>

            <!-- Pakiety (Tiers) -->
            <div class="space-y-4">
              <h3 class="text-xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50 mb-2">Pakiety Usług</h3>
              <p class="text-sm text-zinc-500 dark:text-zinc-400 -mt-3 mb-4">Dodaj warianty cenowe i opcje dla Twojej usługi.</p>

              <FieldArray name="tiers" v-slot="{ fields, push, remove }">
                <div class="space-y-5">
                  <div
                    v-for="(field, index) in fields"
                    :key="field.key"
                    class="p-5 rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900/50 relative group transition-all hover:border-teal-200 dark:hover:border-teal-900"
                  >
                    <div class="flex items-center justify-between mb-4">
                      <h4 class="font-semibold text-teal-600 dark:text-teal-400">Pakiet {{ index + 1 }}</h4>
                      <Button
                        v-if="index !== 0"
                        type="button"
                        variant="ghost"
                        size="sm"
                        class="h-8 text-red-500 hover:text-red-700 hover:bg-red-50 dark:hover:bg-red-950/50"
                        @click="remove(index)"
                      >
                        <Trash2 class="w-4 h-4 mr-2" />
                        Usuń
                      </Button>
                    </div>

                    <div class="grid gap-4">
                      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div class="grid gap-2">
                          <Label class="text-xs text-zinc-500 dark:text-zinc-400 uppercase tracking-wider font-semibold">Nazwa pakietu</Label>
                          <Field
                            :name="`tiers[${index}].name`"
                            as="input"
                            class="flex h-10 w-full rounded-lg border border-zinc-200 dark:border-zinc-800 bg-transparent px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 transition-colors"
                            placeholder="Np. Podstawowy, Premium"
                          />
                          <ErrorMessage :name="`tiers[${index}].name`" class="text-xs text-red-500 font-medium" />
                        </div>

                        <div class="grid gap-2">
                          <Label class="text-xs text-zinc-500 dark:text-zinc-400 uppercase tracking-wider font-semibold">Cena (PLN)</Label>
                          <Field
                            :name="`tiers[${index}].price`"
                            type="number"
                            min="0"
                            as="input"
                            class="flex h-10 w-full rounded-lg border border-zinc-200 dark:border-zinc-800 bg-transparent px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 transition-colors"
                            placeholder="0"
                          />
                          <ErrorMessage :name="`tiers[${index}].price`" class="text-xs text-red-500 font-medium" />
                        </div>
                      </div>

                      <div class="grid gap-2">
                          <Label class="text-xs text-zinc-500 dark:text-zinc-400 uppercase tracking-wider font-semibold">Krótki opis</Label>
                        <Field
                          :name="`tiers[${index}].description`"
                          as="input"
                          class="flex h-10 w-full rounded-lg border border-zinc-200 dark:border-zinc-800 bg-transparent px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 transition-colors"
                          placeholder="Zawartość pakietu..."
                        />
                        <ErrorMessage :name="`tiers[${index}].description`" class="text-xs text-red-500 font-medium" />
                      </div>
                    </div>
                  </div>

                  <Button
                    type="button"
                    variant="outline"
                    class="w-full border-dashed border-2 border-zinc-200 dark:border-zinc-800 hover:border-teal-500 dark:hover:border-teal-500 hover:bg-teal-50 dark:hover:bg-teal-950/20 text-zinc-600 dark:text-zinc-400 hover:text-teal-700 dark:hover:text-teal-400 py-6"
                    @click="push({ name: '', price: 0, description: '' })"
                  >
                    <Plus class="w-5 h-5 mr-2" />
                    Dodaj kolejny pakiet
                  </Button>
                </div>
              </FieldArray>
            </div>
            
            <DialogFooter class="pt-6 mt-6 border-t border-zinc-200 dark:border-zinc-800">
              <Button type="button" variant="ghost" @click="$emit('close')" class="mb-2 sm:mb-0">
                Anuluj
              </Button>
              <Button type="submit" class="bg-teal-600 hover:bg-teal-700 text-white shadow-md font-semibold px-8" :disabled="isSubmitting">
                <span v-if="isSubmitting" class="flex items-center gap-2">
                  <div class="w-4 h-4 rounded-full border-2 border-white border-t-transparent animate-spin"></div>
                  Zapisywanie...
                </span>
                <span v-else>Opublikuj Ogłoszenie</span>
              </Button>
            </DialogFooter>
          </div>
        </Form>
      </ScrollArea>
    </DialogContent>
  </Dialog>
</template>
