<script setup>
import { ref } from "vue";
import { Form, Field, ErrorMessage } from "vee-validate";
import * as yup from "yup";

const props = defineProps(["isOpen"]);
const emit = defineEmits(["close", "switchToRegister"]);

const serverNotification = ref("");

const schema = yup.object({
  email: yup.string().required("Login required"),
  password: yup.string().required("Password required"),
  doNotLogOut: yup.boolean(),
});

const onSubmit = async (values) => {
  serverNotification.value = "";

  try {
    const response = await fetch("/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        Login: values.email,
        Password: values.password,
        doNotLogOut: values.doNotLogOut,
      }),
      credentials: "include",
    });

    if (!response.ok) {
      if (response.status === 400 || response.status === 401) {
        serverNotification.value = "Wrong login or password";
      } else {
        serverNotification.value = "An error occurred. Please try again.";
      }
      return;
    }

    const result = await response.text();

    emit("close");
  } catch (err) {
    console.error("Network Error:", err);
    serverNotification.value = "Connection error.";
  }
};
</script>
<template>
  <Teleport to="body">
    <div v-if="isOpen" class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-content">
        <button type="button" id="cancelButton" @click="$emit('close')">
          ×
        </button>
        <h2>Log in</h2>

        <Form
          :validation-schema="schema"
          :initial-values="{ email: '', password: '', doNotLogOut: false }"
          @submit="onSubmit"
          v-slot="{ errors }"
        >
          <div id="fields-flex">
            <span v-if="serverNotification" class="error-text server-error">
              {{ serverNotification }}
            </span>

            <Field name="email" type="text" placeholder="Login" />
            <ErrorMessage name="email" class="error-text" />

            <hr />

            <Field name="password" type="password" placeholder="Password" />
            <ErrorMessage name="password" class="error-text" />
          </div>

          <label class="checkbox-container">
            <Field name="doNotLogOut" type="checkbox" :value="true" />
            Do not log out
          </label>

          <div id="buttons">
            <button type="submit" id="ButtonModal">Submit</button><br />
            <hr />
            <button
              type="button"
              id="ButtonModal"
              @click="$emit('switchToRegister')"
            >
              Register
            </button>
          </div>
        </Form>
      </div>
    </div>
  </Teleport>
</template>
