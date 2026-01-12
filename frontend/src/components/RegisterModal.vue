<template>
  <Teleport to="body">
    <div v-if="isOpen" class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-content">
        <button type="button" id="cancelButton" @click="$emit('close')">×</button>
        <h2>Register</h2>
        <form @submit.prevent="handleRegister">
          <div id="fields-flex">
            <span v-if="serverNotification" class="error-text server-error">{{ serverNotification }}</span>
              <input type="text" v-model="formData.userName" placeholder="User Name"/>
              <span v-if="errors.userName" class="error-text">{{ errors.userName }}</span> <input type="email" v-model="formData.email" placeholder="Email"/>
              <span v-if="errors.email" class="error-text">{{ errors.email }}</span>

              <input type="text" v-model="formData.firstName" placeholder="First Name"/>
              <span v-if="errors.firstName" class="error-text">{{ errors.firstName }}</span> <input type="text" v-model="formData.lastName" placeholder="Last Name"/>
              <span v-if="errors.lastName" class="error-text">{{ errors.lastName }}</span> <input type="password" v-model="formData.password" placeholder="Password"/>
              <span v-if="errors.password" class="error-text">{{ errors.password }}</span> <input type="password" v-model="formData.confirmPassword" placeholder="Confirm Password"/>
              <span v-if="errors.confirmPassword" class="error-text">{{ errors.confirmPassword }}</span> </div>
            
            <div class="checkbox-group">
              <input type="checkbox" id="IsFreelancer" v-model="formData.isFreelancer" />
              <label for="IsFreelancer"> Is freelancer</label>
          </div>

          <button type="submit" class="btn-main" id="ButtonModal">Register Now</button>
          
        </form>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import {reactive, ref as vueRef} from 'vue';
import { object, string, boolean, ref as yupRef} from 'yup';
const specialChars = "!@#$%^&*()_+-=[]{};':\"\\|,.<>/?";
defineProps(['isOpen']);
const emit = defineEmits(['close', 'switchToLogin']);
const serverNotification = vueRef('');
const errors=vueRef({})
const formData=reactive({
  userName: '',
  email: '',
  firstName: '',
  lastName: '',
  password: '',
  confirmPassword: '',
  isFreelancer: false
})
const schema=object({
  userName: string().required('Username required'),
  email: string().email('Invalid email format').required('Email required'),
  firstName:string().required('First name required'),
  lastName:string().required('Last name required'),
   password: string()
    .min(8, 'Password must contain at least 8 characters')
    .required('Password required')
    .test('special-char', 'Need one special character', (value) => {
      return value ? [...value].some(char => specialChars.includes(char)) : false;
    })
    .matches(/(?=.*[0-9])/, 'Password must contain at least one number'),
  confirmPassword: string().oneOf([yupRef('password')], 'Passwords must match').required('Please confirm your password'),
  isFreelancer: boolean()
});
const handleRegister = async () => {
    try{
    serverNotification.value = '';
    errors.value = {};
    await schema.validate(formData, { abortEarly: false });
    const response = await fetch('/api/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                Login: formData.userName,
                Email: formData.email,
                FirstName: formData.firstName,
                LastName: formData.lastName,
                Password: formData.password,
                isFreelancer: formData.isFreelancer
            }),
        });
        const result = await response.text();
        console.log(response);
        if (!response.ok) {
            if (response.status === 401 || response.status === 409) {
                serverNotification.value = "Username or Email is already taken.";
            }
            else {
              serverNotification.value = "An error occurred. Please try again.";
            }
          return;
        }
        if(response.ok){
        console.log("Success:", result);
        emit('switchToLogin');
        }
    }
    catch (err) {
    if (err.inner){
        err.inner.forEach((error) => {
      errors.value[error.path] = error.message;
    });
}
}
};
</script>
