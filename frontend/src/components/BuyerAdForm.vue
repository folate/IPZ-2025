<script setup>
import * as vue from "vue";
import * as yup from "yup";
import { Form, Field, ErrorMessage } from "vee-validate";

const props = defineProps(["isOpen"]);
const emit = defineEmits(["close"]);

const categoryList = vue.ref([]);
const submitError = vue.ref("");
const isSubmitting = vue.ref(false);

const fetchCategories = async () => {
  try {
    const response = await fetch("/api/category", {
      credentials: "include",
    });
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

const today = new Date();
const todayStr = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, "0")}-${String(
  today.getDate(),
).padStart(2, "0")}`;

const schema = yup.object({
  title: yup
    .string()
    .transform((v) => (typeof v === "string" ? v.trim() : v))
    .required("Title is required")
    .min(3, "Title must be at least 3 characters"),

  description: yup
    .string()
    .transform((v) => (typeof v === "string" ? v.trim() : v))
    .required("Description is required")
    .min(10, "Description must be at least 10 characters"),

  categories: yup.string().required("Please select a category"),

  budget: yup
    .number()
    .typeError("Budget must be a number")
    .required("Budget is required")
    .min(0, "Budget cannot be negative"),

  deadline: yup
    .date()
    .typeError("Deadline is required")
    .required("Deadline is required")
    .min(new Date(todayStr), "Deadline must be today or later"),
});

function toIsoFromDateInput(dateStr) {
  const d = new Date(`${dateStr}T23:59:59`);
  return d.toISOString();
}

const onSubmit = async (values) => {
  submitError.value = "";
  isSubmitting.value = true;

  try {
    const response = await fetch("/api/BuyerAd/create", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({
        Title: values.title,
        Description: values.description,
        Category: values.categories,
        Budget: Number(values.budget),
        Deadline: toIsoFromDateInput(values.deadline),
      }),
    });

    if (!response.ok) {
      const errorText = await response.text();
      submitError.value = `Server error (${response.status}): ${errorText}`;
      return;
    }

    await response.text();
    window.dispatchEvent(new Event("buyerad:created"));
    emit("close");
  } catch (err) {
    submitError.value = "Submission error (network problem).";
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<template>
  <Teleport to="body">
    <div v-if="isOpen" class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-content-adform">
        <button type="button" id="cancelButton" @click="$emit('close')">
          ×
        </button>

        <h2>Create a Listing - Buyer Ad Form</h2>

        <Form
          :validation-schema="schema"
          :initial-values="{
            title: '',
            description: '',
            categories: '',
            budget: 0,
            deadline: todayStr,
          }"
          @submit="onSubmit"
        >
          <div id="fields-flex">
            <div class="field-group">
              <Field name="title" type="text" placeholder="Title" />
              <ErrorMessage name="title" class="error-text" />
            </div>

            <div class="field-group">
              <Field name="description" type="text" placeholder="Description" />
              <ErrorMessage name="description" class="error-text" />
            </div>

            <div class="field-group">
              <Field name="categories" as="select">
                <option value="" disabled>Select Category</option>
                <option
                  v-for="category in categoryList"
                  :key="category.name"
                  :value="category.name"
                >
                  {{ category.name }}
                </option>
              </Field>
              <ErrorMessage name="categories" class="error-text" />
            </div>

            <div class="field-group">
              <Field
                name="budget"
                type="number"
                placeholder="Budget"
                min="0"
                step="1"
              />
              <ErrorMessage name="budget" class="error-text" />
            </div>

            <div class="field-group">
              <Field name="deadline" type="date" :min="todayStr" />
              <ErrorMessage name="deadline" class="error-text" />
            </div>
          </div>

          <p v-if="submitError" class="error-text">
            {{ submitError }}
          </p>

          <div id="buttons">
            <button type="submit" id="submit" :disabled="isSubmitting">
              Submit Listing
            </button>
          </div>
        </Form>
      </div>
    </div>
  </Teleport>
</template>
