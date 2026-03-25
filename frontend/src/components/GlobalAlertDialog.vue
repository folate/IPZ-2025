<script setup>
import { useAlert } from "@/stores/alert";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";

const { state, closeAlert } = useAlert();

function handleOpenChange(open) {
  if (!open) {
    closeAlert();
  }
}
</script>

<template>
  <Dialog :open="state.isOpen" @update:open="handleOpenChange">
    <DialogContent class="sm:max-w-[425px]">
      <DialogHeader>
        <DialogTitle :class="state.variant === 'destructive' ? 'text-red-600' : ''">
          {{ state.title }}
        </DialogTitle>
        <DialogDescription v-if="state.description">
          {{ state.description }}
        </DialogDescription>
      </DialogHeader>
      <DialogFooter>
        <Button 
          type="button" 
          :variant="state.variant === 'destructive' ? 'destructive' : 'default'"
          @click="closeAlert"
        >
          OK
        </Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
