<template>
  <Teleport to="body">
    <div v-if="isOpen" class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-content">
        <button type="button" id="cancelButton" @click="$emit('close')">
          ×
        </button>
        <h2>Register</h2>

        <Form
          :validation-schema="schema"
          :initial-values="{
            userName: '',
            email: '',
            firstName: '',
            lastName: '',
            password: '',
            confirmPassword: '',
            isFreelancer: false,
          }"
          @submit="onSubmit"
        >
          <div id="fields-flex">
            <span v-if="serverNotification" class="error-text server-error">
              {{ serverNotification }}
            </span>

            <Field name="userName" type="text" placeholder="User Name" />
            <ErrorMessage name="userName" class="error-text" />

            <Field name="email" type="email" placeholder="Email" />
            <ErrorMessage name="email" class="error-text" />

            <Field name="firstName" type="text" placeholder="First Name" />
            <ErrorMessage name="firstName" class="error-text" />

            <Field name="lastName" type="text" placeholder="Last Name" />
            <ErrorMessage name="lastName" class="error-text" />

            <Field name="password" type="password" placeholder="Password" />
            <ErrorMessage name="password" class="error-text" />

            <Field
              name="confirmPassword"
              type="password"
              placeholder="Confirm Password"
            />
            <ErrorMessage name="confirmPassword" class="error-text" />
          </div>

          <div class="checkbox-group">
            <Field
              name="isFreelancer"
              type="checkbox"
              :value="true"
              id="IsFreelancer"
            />
            <label for="IsFreelancer" class="no-select"> Is freelancer</label>
          </div>

          <button type="submit" class="btn-main" id="ButtonModal">
            Register Now
          </button>
        </Form>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { ref } from "vue";
import { Form, Field, ErrorMessage } from "vee-validate";
import * as yup from "yup";

const props = defineProps(["isOpen"]);
const emit = defineEmits(["close", "switchToLogin"]);

const serverNotification = ref("");
const specialChars = "!@#$%^&*()_+-=[]{};':\"\\|,.<>/?";

const schema = yup.object({
  userName: yup.string().required("Username required"),
  email: yup.string().email("Invalid email format").required("Email required"),
  firstName: yup.string().required("First name required"),
  lastName: yup.string().required("Last name required"),
  password: yup
    .string()
    .min(8, "Password must contain at least 8 characters")
    .required("Password required")
    .test("special-char", "Need one special character", (value) => {
      return value
        ? [...value].some((char) => specialChars.includes(char))
        : false;
    })
    .matches(/(?=.*[0-9])/, "Password must contain at least one number"),
  confirmPassword: yup
    .string()
    .oneOf([yup.ref("password")], "Passwords must match")
    .required("Please confirm your password"),
  isFreelancer: yup.boolean(),
});

const onSubmit = async (values) => {
  serverNotification.value = "";

  try {
    const response = await fetch("/api/auth/register", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        Login: values.userName,
        Email: values.email,
        FirstName: values.firstName,
        LastName: values.lastName,
        Password: values.password,
        isFreelancer: values.isFreelancer,
      }),
    });

    if (!response.ok) {
      if (response.status === 401 || response.status === 409) {
        serverNotification.value = "Username or Email is already taken.";
      } else {
        serverNotification.value = "An error occurred. Please try again.";
      }
      return;
    }

    const result = await response.text();
    //console.log("Success:", result);
    emit("close");
  } catch (err) {
    console.error("Network error:", err);
    serverNotification.value = "Connection error. Please check your internet.";
  }
};
</script>
