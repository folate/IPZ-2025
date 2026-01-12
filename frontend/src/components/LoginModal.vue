<template>
  <Teleport to="body">
    <div v-if="isOpen" class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-content">
        <button type="button" id="cancelButton" @click="$emit('close')">×</button>
        <h2>Log in</h2>
        <form @submit.prevent="handleLogin">
            <div id="fields-flex">
          <span v-if="serverNotification" class="error-text server-error">{{ serverNotification }}</span>
          <input type="text" v-model="formData.email" placeholder="Login" />
          <span v-if="errors.email" class="error-text">{{ errors.email }}</span>
          <hr>
          <input type="password" v-model="formData.password" placeholder="Password"/>
          <span v-if="errors.password" class="error-text">{{ errors.password }}</span>
          </div>
          <label class="checkbox-container">
            <input type="checkbox" v-model="formData.doNotLogOut"/>Do not log out</label>
          <div id="buttons">
            <button type="submit" id="ButtonModal">Submit</button><br>
          <hr>
          <button type="button" id="ButtonModal" @click="$emit('switchToRegister')">Register</button>
          </div>
        </form>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import {reactive, ref as vueRef} from 'vue';
import { object, string, boolean} from 'yup';
const serverNotification = vueRef('');
const formData=reactive({
  email: '',
  password: '',
  doNotLogOut: false
})
const specialChars = "!@#$%^&*()_+-=[]{};':\"\\|,.<>/?";
const errors=vueRef({})
const schema=object({
  email: string().required('Login required'),
  password: string().required('Password required'),
  doNotLogOut: boolean()
});

defineProps(['isOpen']);
defineEmits(['close','switchToRegister']);

const handleLogin = async () => {
    try {
        errors.value = {};
        await schema.validate(formData, { abortEarly: false });
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                login: formData.email,
                password: formData.password
            }),
            credentials: 'include'
        });
        if (!response.ok) {
            if (response.status === 401) {
              serverNotification.value = "Wrong login or password";
          } 
            else {
              serverNotification.value = "An error occurred. Please try again.";
          }
          return;
        }
        const result = await response.text();
        if(response.ok){
        console.log("Success:", result);
        emit('close');
        }
    } catch (err) {
        console.error("Caught Error:", err); 
        if (err.inner) {
            err.inner.forEach((error) => {
                errors.value[error.path] = error.message;
            });
        }
    }
};
</script>