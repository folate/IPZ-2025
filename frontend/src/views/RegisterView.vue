<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { Form, Field, ErrorMessage } from "vee-validate";
import * as yup from "yup";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

const router = useRouter();

const serverNotification = ref("");
const isLoading = ref(false);
const specialChars = "!@#$%^&*()_+-=[]{};':\"\\|,.<>/?";

const schema = yup.object({
  userName: yup.string().required("Podaj nazwę użytkownika"),
  email: yup.string().email("Nieprawidłowy format email").required("Email jest wymagany"),
  firstName: yup.string().required("Podaj imię"),
  lastName: yup.string().required("Podaj nazwisko"),
  password: yup
    .string()
    .min(8, "Hasło musi mieć co najmniej 8 znaków")
    .required("Hasło jest wymagane")
    .test("special-char", "Wymagany jeden znak specjalny", (value) => {
      return value
        ? [...value].some((char) => specialChars.includes(char))
        : false;
    })
    .matches(/(?=.*[0-9])/, "Hasło musi zawierać co najmniej jedną cyfrę"),
  confirmPassword: yup
    .string()
    .oneOf([yup.ref("password")], "Hasła muszą być identyczne")
    .required("Potwierdź hasło"),
  isFreelancer: yup.boolean(),
});

const onSubmit = async (values) => {
  serverNotification.value = "";
  isLoading.value = true;
  
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
        isFreelancer: values.isFreelancer || false,
      }),
    });

    if (!response.ok) {
      if (response.status === 401 || response.status === 409) {
        serverNotification.value = "Nazwa użytkownika lub Email są już zajęte.";
      } else {
        serverNotification.value = "Wystąpił błąd. Spróbuj ponownie.";
      }
      return;
    }

    // Success
    router.push("/login");
  } catch (err) {
    console.error("Network error:", err);
    serverNotification.value = "Błąd połączenia. Sprawdź swoje połączenie z siecią.";
  } finally {
    isLoading.value = false;
  }
};
</script>

<template>
  <div class="flex min-h-svh w-full items-center justify-center p-6 md:p-10 bg-zinc-50 dark:bg-zinc-950">
    <div class="w-full max-w-5xl">
      <Card class="overflow-hidden p-0 border-zinc-200 dark:border-zinc-800 shadow-xl shadow-teal-900/5">
        <CardContent class="grid p-0 md:grid-cols-2">
          
          <!-- Left side: Form -->
          <div class="p-6 md:p-8 flex flex-col justify-center">
            <div class="flex flex-col items-center gap-2 text-center mb-8">
              <h1 class="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">Stwórz konto</h1>
              <p class="text-zinc-500 dark:text-zinc-400">
                Wypełnij poniższe dane, aby rozpocząć
              </p>
            </div>
            
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
              <div class="flex flex-col gap-2">
                <!-- Error Message -->
                <div v-if="serverNotification" class="p-3 mb-2 text-sm bg-red-50 dark:bg-red-900/20 text-red-600 dark:text-red-400 rounded-lg border border-red-100 dark:border-red-900/30 font-medium text-center">
                  {{ serverNotification }}
                </div>

                <div class="grid grid-cols-1 sm:grid-cols-2 gap-x-4 gap-y-1">
                  <div class="grid gap-1">
                    <Label for="firstName" class="text-zinc-700 dark:text-zinc-300">Imię</Label>
                    <Field name="firstName" as="input" class="flex h-11 w-full rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 disabled:cursor-not-allowed disabled:opacity-50 transition-colors" placeholder="Jan" />
                    <div class="h-4"><ErrorMessage name="firstName" class="text-xs text-red-500 font-medium block" /></div>
                  </div>
                  <div class="grid gap-1">
                    <Label for="lastName" class="text-zinc-700 dark:text-zinc-300">Nazwisko</Label>
                    <Field name="lastName" as="input" class="flex h-11 w-full rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 disabled:cursor-not-allowed disabled:opacity-50 transition-colors" placeholder="Kowalski" />
                    <div class="h-4"><ErrorMessage name="lastName" class="text-xs text-red-500 font-medium block" /></div>
                  </div>
                </div>

                <div class="grid gap-1">
                  <Label for="userName" class="text-zinc-700 dark:text-zinc-300">Nazwa użytkownika</Label>
                  <Field name="userName" as="input" class="flex h-11 w-full rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 disabled:cursor-not-allowed disabled:opacity-50 transition-colors" placeholder="jankowalski99" />
                  <div class="h-4"><ErrorMessage name="userName" class="text-xs text-red-500 font-medium block" /></div>
                </div>

                <div class="grid gap-1">
                  <Label for="email" class="text-zinc-700 dark:text-zinc-300">Adres Email</Label>
                  <Field name="email" as="input" type="email" class="flex h-11 w-full rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 disabled:cursor-not-allowed disabled:opacity-50 transition-colors" placeholder="jan@example.com" />
                  <div class="h-4"><ErrorMessage name="email" class="text-xs text-red-500 font-medium block" /></div>
                </div>
                
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-x-4 gap-y-1">
                  <div class="grid gap-1">
                    <Label for="password" class="text-zinc-700 dark:text-zinc-300">Hasło</Label>
                    <Field name="password" as="input" type="password" class="flex h-11 w-full rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 disabled:cursor-not-allowed disabled:opacity-50 transition-colors" />
                    <div class="h-4"><ErrorMessage name="password" class="text-xs text-red-500 font-medium block" /></div>
                  </div>
                  <div class="grid gap-1">
                    <Label for="confirmPassword" class="text-zinc-700 dark:text-zinc-300">Potwierdź hasło</Label>
                    <Field name="confirmPassword" as="input" type="password" class="flex h-11 w-full rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-500 disabled:cursor-not-allowed disabled:opacity-50 transition-colors" />
                    <div class="h-4"><ErrorMessage name="confirmPassword" class="text-xs text-red-500 font-medium block" /></div>
                  </div>
                </div>

                <div class="flex items-center space-x-2 mt-2 mb-2">
                  <Field name="isFreelancer" type="checkbox" :value="true" :unchecked-value="false" v-slot="{ field, handleChange }">
                    <div class="flex items-center gap-2">
                      <Checkbox
                        id="isFreelancer"
                        :checked="field.checked"
                        @update:checked="handleChange"
                      />
                      <label 
                        for="isFreelancer" 
                        class="text-sm font-medium leading-none cursor-pointer text-zinc-900 dark:text-zinc-300"
                      >
                        Chcę oferować usługi jako Freelancer
                      </label>
                    </div>
                  </Field>
                </div>

                <Button type="submit" class="w-full h-11 rounded-xl bg-teal-600 hover:bg-teal-700 text-white shadow-md font-semibold text-base transition-colors mt-2" :disabled="isLoading">
                  <span v-if="isLoading" class="flex items-center gap-2">
                    <div class="w-4 h-4 rounded-full border-2 border-white border-t-transparent animate-spin"></div>
                    Rejestracja...
                  </span>
                  <span v-else>Zarejestruj się</span>
                </Button>
              </div>
              <div class="mt-4 text-center text-sm text-zinc-500 dark:text-zinc-400">
                Masz już konto?
                <router-link to="/login" class="font-medium inline-block text-teal-600 dark:text-teal-400 hover:text-teal-700 hover:underline ml-1">
                  Zaloguj się
                </router-link>
              </div>
            </Form>
          </div>

          <!-- Right side: Image -->
          <div class="bg-zinc-100 dark:bg-zinc-900 relative hidden md:block">
            <img
              src="https://images.unsplash.com/photo-1522071820081-009f0129c71c?q=80&w=1500&auto=format&fit=crop"
              alt="Community"
              class="absolute inset-0 h-full w-full object-cover dark:brightness-[0.7] dark:grayscale-[0.3]"
            />
            <div class="absolute inset-0 bg-teal-900/10 mix-blend-multiply dark:bg-teal-900/40"></div>
          </div>

        </CardContent>
      </Card>
      
      <div class="mt-6 text-center text-sm text-zinc-500 dark:text-zinc-400">
        Klikając kontynuuj, zgadzasz się na nasze <a href="#" class="underline hover:text-teal-600">Warunki świadczenia usług</a>
        i <a href="#" class="underline hover:text-teal-600">Politykę Prywatności</a>.
      </div>
    </div>
  </div>
</template>
