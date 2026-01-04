<template>
  <Teleport to="body">
    <div v-if="isOpen" class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-content">
        <button type="button" id="cancelButton" @click="$emit('close')">×</button>
        <h2>Register</h2>
        <form @submit.prevent="handleRegister">
          <div id="fields-flex">
            <div id="fields-flex">
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
          </div>

          <button type="submit" class="btn-main">Register Now</button>
          
        </form>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import {reactive, ref as vueRef} from 'vue';
import { object, string, boolean, ref as yupRef} from 'yup';

defineProps(['isOpen']);
const emit=defineEmits(['close']);

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
  password:  string().min(6, 'Password must contain at least 6 characters').required('Password required'),
  confirmPassword: string().oneOf([yupRef('password')], 'Passwords must match').required('Please confirm your password'),
  isFreelancer: boolean()
});
const handleRegister = async () => {
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