<template>
  <Teleport to="body">
    <div v-if="isOpen" class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-content">
        <button type="button" id="cancelButton" @click="$emit('close')">×</button>
        <h2>Create an Account</h2>
        <form @submit.prevent="handleLogin">
            <div id="fields-flex">
          <input type="email" v-model="formData.email" placeholder="Email" />
          <span v-if="errors.email" class="error-text">{{ errors.email }}</span>
          <hr>
          <input type="password" v-model="formData.password" placeholder="Password"/>
          <span v-if="errors.password" class="error-text">{{ errors.password }}</span>
          </div>
          <label class="checkbox-container">
            <input type="checkbox" v-model="formData.doNotLogOut"/>Do not log out</label>
          <div id="buttons">
            <button type="submit">Submit</button><br>
          <hr>
          <button type="button" @click="$emit('switchToRegister')">Register</button>
          </div>
        </form>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import {reactive, ref as vueRef} from 'vue';
import { object, string, boolean} from 'yup';
const formData=reactive({
  email: '',
  password: '',
  doNotLogOut: false
})
const errors=vueRef({})
const schema=object({
  email: string().email('Invalid email format').required('Email required'),
  password:  string().min(6, 'Password must contain at least 6 characters').required('Password required'),
  doNotLogOut: boolean()
});

defineProps(['isOpen']);
defineEmits(['close','switchToRegister']);

const handleLogin = async () => {
    try{
    errors.value = {};
    await schema.validate(formData, { abortEarly: false });
    console.log("Registration Data:", formData);
    }
    catch (err) {
    if (err.inner){
        err.inner.forEach((error) => {
      errors.value[error.path] = error.message;
    });
}
    
    console.log("Logging in...");
}
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0; left: 0;
  width: 100vw; height: 100vh;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(10px);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}
.modal-content {
    text-align: center;
    box-shadow: 10px 10px 10px 10px rgba(0, 0, 0, 0.792);
  background: white;
  padding: 2rem;
  border-radius: 8px;
  color: black;
  width: 20%;
  height: auto;
}
div#fields-flex{
    display: flex;
    flex-direction: column;
    margin:5%;
    justify-content: space-between;
    width:90%;
    border-radius: 5px;
    border-style: solid;
    border-width: 2px;
    border-color: rgb(186, 187, 188);
    padding:10px;
}
div#fields-flex input, button{
    border: none;
    background-color: rgb(216, 216, 216);
    padding: 10px;
    margin: 10px;
    border-radius: 5px;
}
button#cancelButton{
    text-align: right;
    display: flex;
}
button:hover{
    cursor: pointer;
}
div#buttons{
    text-align: center;
    width: 80%;
    display: flex;
    flex-direction: column;
    margin:5% auto;
}

.error-text {
  color: #d93025;
  font-size: 0.75rem;
  margin-top: -5px;
  margin-bottom: 5px;
  text-align: left;
  padding-left: 10px;
}
</style>