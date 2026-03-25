import { reactive } from "vue";

const state = reactive({
  isOpen: false,
  title: "",
  description: "",
  variant: "default", // default, destructive
});

export function useAlert() {
  function showAlert(title, description = "", variant = "default") {
    state.title = title;
    state.description = description;
    state.variant = variant;
    state.isOpen = true;
  }

  function closeAlert() {
    state.isOpen = false;
  }

  return {
    state,
    showAlert,
    closeAlert,
  };
}
