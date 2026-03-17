<script setup>
import * as vue from "vue";
import * as yup from "yup";
import { Form, Field, ErrorMessage, FieldArray } from "vee-validate";
import axios from "axios";
defineProps(["isOpen"]);
const emit = defineEmits(["close"]);
const categoryList = vue.ref([]);
const fileUpload = vue.ref(null);
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
  title: yup.string().required("Title is required"),
  description: yup.string().required("Description is required"),
  categories: yup.string().required("Category is required"),
  tiers: yup
    .array()
    .of(
      yup.object({
        name: yup.string().required("Tier Name required"),
        price: yup
          .number()
          .typeError("Price must be a number")
          .required("Price is required")
          .positive("Price must be positive"),
        description: yup.string().required("Tier Description required"),
      }),
    )
    .min(1, "At least one tier is required"),
});

const onSubmit = async (values, { setFieldError }) => {
  try {
    const ImageFiles = fileUpload.value?.files;
    const formData = new FormData();
    formData.append("Title", values.title);
    formData.append("Description", values.description);
    formData.append("Category", values.categories);

    if (ImageFiles && ImageFiles.length > 0) {
      Array.from(ImageFiles).forEach((file) => {
        formData.append("Photos", file);
      });
    }

    values.tiers.forEach((tier, index) => {
      formData.append(`Gigs[${index}].TierName`, tier.name);
      formData.append(`Gigs[${index}].TierDescription`, tier.description);
      formData.append(`Gigs[${index}].Price`, tier.price);
    });
    await axios.post("/api/sellerad/create", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    //console.log("YESSS");
    emit("close");
  } catch (err) {
    if (err.response?.status === 400) {
      const validationErrors = err.response.data.errors;
      setFieldErrors(validationErrors);
    }
    if (err.response?.status === 413) {
      setFieldError("images", "File size too large.");
    }
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
        <h2>Create a Listing</h2>

        <Form
          :validation-schema="schema"
          :initial-values="{ tiers: [{ name: '', price: 0, description: '' }] }"
          @submit="onSubmit"
          v-slot="{ values }"
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
              <input type="file" accept="image/*" multiple ref="fileUpload" />
              <ErrorMessage name="images" class="error-text" />
            </div>
            <hr />
            <h3>Tiers</h3>

            <FieldArray name="tiers" v-slot="{ fields, push, remove }">
              <div
                v-for="(field, index) in fields"
                :key="field.key"
                class="tier-section"
              >
                <h4>Tier {{ index + 1 }}</h4>

                <div class="field-group">
                  <Field
                    :name="`tiers[${index}].name`"
                    placeholder="Tier Name"
                  />
                  <ErrorMessage
                    :name="`tiers[${index}].name`"
                    class="error-text"
                  />
                </div>

                <div class="field-group">
                  <Field
                    :name="`tiers[${index}].price`"
                    type="number"
                    placeholder="Price"
                    min="0"
                  />
                  <ErrorMessage
                    :name="`tiers[${index}].price`"
                    class="error-text"
                  />
                </div>

                <div class="field-group">
                  <Field
                    :name="`tiers[${index}].description`"
                    placeholder="Tier Description"
                  />
                  <ErrorMessage
                    :name="`tiers[${index}].description`"
                    class="error-text"
                  />
                </div>

                <button
                  v-if="index !== 0"
                  type="button"
                  id="submit"
                  class="remove-btn"
                  @click="remove(index)"
                >
                  Remove Tier
                </button>
              </div>

              <button
                type="button"
                class="add-btn"
                id="submit"
                @click="push({ name: '', price: 0, description: '' })"
              >
                + Add Tier
              </button>
            </FieldArray>
          </div>

          <div id="buttons">
            <button type="submit" id="submit">Submit Listing</button>
          </div>
        </Form>
      </div>
    </div>
  </Teleport>
</template>
