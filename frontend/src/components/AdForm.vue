<template>
  <Teleport to="body">
    <div v-if="isOpen" class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-content">
        <button type="button" id="cancelButton" @click="$emit('close')">
          ×
        </button>
        <h2>Create a Listing</h2>

        <Form
          :validation-schema="schema"
          @submit="onSubmit"
          v-slot="{ values, errors }"
        >
          <div id="fields-flex">
            <Field name="title" type="text" placeholder="Title" />
            <ErrorMessage name="title" class="error-text" />

            <Field
              name="description"
              as="input"
              type="text"
              placeholder="Description"
            />
            <ErrorMessage name="description" class="error-text" />

            <Field name="categories" as="select">
              <option value="" disabled>Select Category</option>
              <option value="cat1">cat1</option>
              <option value="cat2">cat2</option>
            </Field>
            <ErrorMessage name="categories" class="error-text" />

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
                    placeholder="Tier Price"
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
                  type="button"
                  @click="remove(index)"
                  v-if="fields.length > 1"
                  class="remove-btn"
                >
                  Remove Tier
                </button>
              </div>

              <button
                type="button"
                @click="push({ name: '', price: 0, description: '' })"
                class="add-btn"
              >
                + Add Tier
              </button>
            </FieldArray>
          </div>

          <div id="buttons">
            <button type="submit">Submit Listing</button>
          </div>
        </Form>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import * as vue from "vue";
import * as yup from "yup";
import { Form, Field, ErrorMessage, FieldArray } from "vee-validate";

defineProps(["isOpen"]);
const emit = defineEmits(["close"]);

const schema = yup.object({
  title: yup.string().required("Title is required"),
  description: yup.string().required("Description is required"),
  categories: yup.string().required("Please select a category"),
  tiers: yup
    .array()
    .of(
      yup.object({
        name: yup.string().required("Tier Name required"),
        price: yup
          .number()
          .typeError("Price must be a number")
          .required("Price required")
          .positive("Price must be positive"),
        description: yup.string().required("Tier Description required"),
      })
    )
    .min(1, "At least one tier is required"),
});

const onSubmit = async (values) => {
  try {
    const gigsPayload = values.tiers.map((tier) => ({
      TierName: tier.name,
      TierDescription: tier.description,
      Price: tier.price,
    }));

    const response = await fetch("/api/ad/create", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        Title: values.title,
        Description: values.description,
        Category: values.categories,
        Gigs: gigsPayload,
        // If you handle photos later, add them here
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
  }
};
</script>
