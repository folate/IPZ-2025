<template>
  <div class="bg-zinc-50 dark:bg-zinc-950 py-8 min-h-[calc(100vh-64px)]">
    <Container>
      <div class="flex h-[calc(100vh-128px)] min-h-[600px] bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-2xl shadow-xl overflow-hidden">
        <!-- Sidebar -->
    <div class="w-full md:w-80 lg:w-96 border-r border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 flex flex-col transition-all duration-300" :class="{'hidden md:flex': state.activeConversationId}">
      <div class="p-4 border-b border-zinc-200 dark:border-zinc-800 flex items-center justify-between sticky top-0 bg-white/95 dark:bg-zinc-900/95 backdrop-blur z-10">
        <h2 class="text-xl font-extrabold text-zinc-900 dark:text-zinc-50 flex items-center gap-2">
          <MessageCircle class="h-6 w-6 text-teal-600" /> Wiadomości
        </h2>
      </div>
      
      <div class="flex-1 overflow-y-auto w-full custom-scrollbar">
        <div v-if="state.conversations.length === 0" class="p-8 text-center flex flex-col items-center justify-center h-full text-zinc-500 dark:text-zinc-400">
           <MessageSquare class="h-12 w-12 text-zinc-300 dark:text-zinc-700 mb-4" />
           <p class="font-medium text-lg">Brak wiadomości</p>
           <p class="text-sm mt-1">Zacznij konwersację z profilu zleceniodawcy lub freelancera.</p>
        </div>
        
        <div v-for="conv in state.conversations" :key="conv.id" 
             @click="selectConversation(conv.id)"
             role="button"
             tabindex="0"
             class="p-4 border-b border-zinc-100 dark:border-zinc-800/50 cursor-pointer transition-all hover:bg-zinc-50 dark:hover:bg-zinc-800/50 group outline-none focus-visible:ring-2 focus-visible:ring-teal-500"
             :class="state.activeConversationId === conv.id ? 'bg-teal-50/50 dark:bg-teal-900/20 border-l-4 border-l-teal-600' : 'border-l-4 border-l-transparent'">
          <div class="flex gap-3 items-center">
            
            <Avatar class="h-12 w-12 border border-zinc-200 dark:border-zinc-700">
              <AvatarFallback class="bg-zinc-100 dark:bg-zinc-800 text-teal-700 dark:text-teal-400 font-bold text-lg">
                {{ getInitials(conv.otherUserName) }}
              </AvatarFallback>
            </Avatar>

            <div class="flex-1 min-w-0">
              <div class="flex justify-between items-baseline mb-1">
                <h3 class="font-bold text-zinc-900 dark:text-zinc-100 truncate pr-2 group-hover:text-teal-700 dark:group-hover:text-teal-400 transition-colors">
                  {{ conv.otherUserName }}
                </h3>
                <span class="text-[11px] font-medium text-zinc-400 dark:text-zinc-500 whitespace-nowrap">
                  {{ formatDate(conv.lastMessageAt) }}
                </span>
              </div>
              <div class="flex justify-between items-center">
                <p class="text-sm text-zinc-500 dark:text-zinc-400 truncate pr-2">
                  <span v-if="state.typingUsers[conv.id]" class="text-teal-600 dark:text-teal-400 italic flex items-center gap-1 font-medium">
                    Pisze<span class="typing-dots"></span>
                  </span>
                  <span v-else-if="conv.lastMessagePreview">{{ conv.lastMessagePreview }}</span>
                  <span v-else class="italic">Rozpoczęto nową konwersację</span>
                </p>
                
                <Badge v-if="conv.unreadCount > 0" variant="default" class="bg-teal-600 hover:bg-teal-700 text-white font-bold h-5 min-w-5 shrink-0 flex items-center justify-center px-1.5 rounded-full">
                  {{ conv.unreadCount }}
                </Badge>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Chat Window -->
    <div class="flex-1 flex flex-col bg-zinc-50/50 dark:bg-zinc-950/50 relative" :class="{'hidden md:flex': !state.activeConversationId}">
      <template v-if="state.activeConversationId">
        <!-- Header -->
        <div class="h-16 px-6 border-b border-zinc-200 dark:border-zinc-800 bg-white/80 dark:bg-zinc-900/80 backdrop-blur-md flex items-center sticky top-0 z-10 gap-4 shadow-sm">
          <Button variant="ghost" size="icon" class="md:hidden -ml-2 text-zinc-500" @click="state.activeConversationId = null">
            <ArrowLeft class="h-5 w-5" />
          </Button>
          
          <Avatar class="h-10 w-10 border border-zinc-200 dark:border-zinc-700">
            <AvatarFallback class="bg-zinc-100 dark:bg-zinc-800 text-teal-700 dark:text-teal-400 font-bold">
              {{ getInitials(activeConversationName) }}
            </AvatarFallback>
          </Avatar>
          
          <div class="flex flex-col">
             <h3 class="font-bold text-lg text-zinc-900 dark:text-zinc-50 leading-tight">{{ activeConversationName }}</h3>
             <span class="text-xs font-medium text-teal-600 dark:text-teal-400 flex items-center gap-1" v-if="state.typingUsers[state.activeConversationId]">
                pisze teraz...
             </span>
          </div>
        </div>

        <!-- Messages Area -->
        <div class="flex-1 overflow-y-auto p-4 md:p-6 flex flex-col gap-4 custom-scrollbar" ref="messagesContainer">
          <div v-for="(msg, index) in state.messages" :key="msg.id" 
               class="flex flex-col group"
               :class="[msg.senderId === currentUserId ? 'items-end' : 'items-start', isNewDay(index) ? 'mt-6' : 'mt-1']">
            
            <div v-if="isNewDay(index)" class="self-center mb-6 mt-2">
              <Badge variant="secondary" class="bg-zinc-200/50 dark:bg-zinc-800/50 text-zinc-500 dark:text-zinc-400 font-medium px-3 py-1 rounded-full text-xs">
                {{ formatDayDivider(msg.sentAt) }}
              </Badge>
            </div>

            <div class="flex max-w-[85%] md:max-w-[75%]" :class="msg.senderId === currentUserId ? 'flex-row-reverse' : 'flex-row'">
              
              <div class="px-5 py-3 shadow-sm relative"
                   :class="[
                     msg.senderId === currentUserId 
                       ? 'bg-teal-600 text-white rounded-2xl rounded-tr-sm' 
                       : 'bg-white dark:bg-zinc-900 text-zinc-900 dark:text-zinc-100 rounded-2xl rounded-tl-sm border border-zinc-200/50 dark:border-zinc-800/50'
                   ]">
                <p class="text-[15px] leading-relaxed break-words whitespace-pre-wrap">{{ msg.content }}</p>
                
                <div class="text-[10px] font-medium flex items-center justify-end gap-1 mt-1 opacity-70 select-none"
                     :class="msg.senderId === currentUserId ? 'text-teal-100' : 'text-zinc-500'">
                  <span>{{ formatTime(msg.sentAt) }}</span>
                  <CheckCheck v-if="msg.senderId === currentUserId && msg.isRead" class="h-3 w-3 ml-0.5 text-teal-200" />
                  <Check v-else-if="msg.senderId === currentUserId" class="h-3 w-3 ml-0.5" />
                </div>
              </div>

            </div>
          </div>
        </div>

        <!-- Input Area -->
        <div class="p-4 bg-white/80 dark:bg-zinc-900/80 backdrop-blur-md border-t border-zinc-200 dark:border-zinc-800 sticky bottom-0">
          <form @submit.prevent="send" class="flex items-end gap-2 max-w-4xl mx-auto">
            <div class="flex-1 relative bg-zinc-100 dark:bg-zinc-950 rounded-2xl border border-zinc-200 dark:border-zinc-800 focus-within:ring-2 focus-within:ring-teal-500/50 focus-within:border-teal-500 transition-all overflow-hidden flex items-end">
              <Textarea 
                v-model="newMessage"
                @keydown.enter.prevent="handleEnter"
                @input="onTyping"
                placeholder="Napisz wiadomość..." 
                class="min-h-[52px] max-h-32 border-0 bg-transparent focus-visible:ring-0 resize-none py-3.5 px-4 text-base"
                rows="1"
              />
            </div>
            
            <Button 
                type="submit"
                :disabled="!newMessage.trim()"
                size="icon"
                class="h-[52px] w-[52px] rounded-2xl bg-teal-600 hover:bg-teal-700 text-white shadow-md disabled:opacity-50 transition-all shrink-0">
              <Send class="h-5 w-5 ml-1" />
            </Button>
          </form>
          <div class="text-center mt-2 max-w-4xl mx-auto hidden md:block">
             <span class="text-[10px] text-zinc-400 font-medium">Naciśnij Enter aby wysłać, Shift + Enter dla nowej linii</span>
          </div>
        </div>
      </template>

      <!-- Empty State -->
      <div v-else class="flex-1 flex items-center justify-center flex-col p-8 text-center animate-in fade-in zoom-in duration-500">
        <div class="h-24 w-24 bg-teal-50 dark:bg-teal-900/20 rounded-full flex items-center justify-center mb-6 border border-teal-100 dark:border-teal-900/30">
          <MessageCircle class="h-12 w-12 text-teal-600 dark:text-teal-400" />
        </div>
        <h3 class="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-2">Twoje Wiadomości</h3>
        <p class="text-zinc-500 dark:text-zinc-400 max-w-sm mx-auto text-lg">
          Wybierz konwersację z listy po lewej stronie, aby przeglądać historię lub wysłać nową wiadomość.
        </p>
      </div>
    </div>
      </div>
    </Container>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch, nextTick } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useChat } from '@/stores/chat';
import { useAuth } from '@/stores/auth';
import { format, isSameDay, isToday, isYesterday } from 'date-fns';
import { pl } from 'date-fns/locale';

import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Badge } from '@/components/ui/badge';
import Container from '@/components/ui/Container.vue';
import { MessageCircle, MessageSquare, Send, ArrowLeft, Check, CheckCheck } from 'lucide-vue-next';

const route = useRoute();
const router = useRouter();
const chat = useChat();
const authStore = useAuth();
const { state } = chat;

const currentUserId = computed(() => authStore.state.user?.id);
const newMessage = ref('');
const messagesContainer = ref(null);

const activeConversationName = computed(() => {
  const conv = state.conversations.find(c => c.id === state.activeConversationId);
  return conv ? conv.otherUserName : '';
});

const targetUserId = computed(() => {
  const conv = state.conversations.find(c => c.id === state.activeConversationId);
  return conv ? conv.otherUserId : null;
});

onMounted(async () => {
  if (!authStore.state.user) {
    await authStore.initAuth();
  }
  
  await chat.connect();
  await chat.loadConversations();

  if (route.params.conversationId) {
    const id = parseInt(route.params.conversationId);
    selectConversation(id);
  }
});

watch(() => route.params.conversationId, (newId) => {
  if (newId) {
    const id = parseInt(newId);
    if (id !== state.activeConversationId) {
      selectConversation(id);
    }
  } else {
    state.activeConversationId = null;
  }
});

watch(() => state.messages.length, () => {
  scrollToBottom();
});

function selectConversation(id) {
  if (route.params.conversationId !== String(id)) {
    router.push(`/chat/${id}`);
  }
  chat.loadMessages(id);
  scrollToBottom();
}

function handleEnter(e) {
  if (!e.shiftKey) {
    send();
  }
}

async function send() {
  if (!newMessage.value.trim() || !state.activeConversationId || !targetUserId.value) return;
  
  const content = newMessage.value;
  newMessage.value = ''; // clear immediately for UX
  await chat.sendMessage(state.activeConversationId, targetUserId.value, content);
}

function onTyping() {
  if (state.activeConversationId && targetUserId.value) {
    chat.sendTyping(state.activeConversationId, targetUserId.value);
  }
}

function scrollToBottom() {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    }
  });
}

// Helpers
function getInitials(name) {
  if (!name) return '?';
  return name.substring(0, 2).toUpperCase();
}

function formatDate(dateStr) {
  if (!dateStr) return '';
  const date = new Date(dateStr);
  if (isToday(date)) return format(date, 'HH:mm');
  if (isYesterday(date)) return 'Wczoraj';
  return format(date, 'd MMM', { locale: pl });
}

function formatDayDivider(dateStr) {
  if (!dateStr) return '';
  const date = new Date(dateStr);
  if (isToday(date)) return 'Dzisiaj';
  if (isYesterday(date)) return 'Wczoraj';
  return format(date, 'd MMMM yyyy', { locale: pl });
}

function formatTime(dateStr) {
  if (!dateStr) return '';
  const date = new Date(dateStr);
  return format(date, 'HH:mm');
}

function isNewDay(index) {
  if (index === 0) return true;
  const currentMsgDate = new Date(state.messages[index].sentAt);
  const prevMsgDate = new Date(state.messages[index - 1].sentAt);
  return !isSameDay(currentMsgDate, prevMsgDate);
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background-color: rgba(161, 161, 170, 0.3);
  border-radius: 20px;
}
.dark .custom-scrollbar::-webkit-scrollbar-thumb {
  background-color: rgba(82, 82, 91, 0.5);
}

.typing-dots::after {
  content: '.';
  animation: typing 1.5s infinite steps(4, end);
}

@keyframes typing {
  0%, 20% { content: '.'; }
  40% { content: '..'; }
  60%, 100% { content: '...'; }
}
</style>
