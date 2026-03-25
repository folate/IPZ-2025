import { reactive, computed } from "vue";
import * as signalR from "@microsoft/signalr";

const state = reactive({
  connection: null,
  conversations: [],
  activeConversationId: null,
  messages: [],
  typingUsers: {}, // { conversationId: { timeoutId } }
});

export function useChat() {
  const isConnected = computed(() => state.connection?.state === signalR.HubConnectionState.Connected);
  
  async function connect() {
    if (state.connection) return;

    state.connection = new signalR.HubConnectionBuilder()
      .withUrl(window.location.origin + "/chathub")
      .withAutomaticReconnect()
      .build();

    state.connection.on("ReceiveMessage", (message) => {
      // If it's for current active conversation, add it
      if (message.conversationId === state.activeConversationId) {
        state.messages.push(message);
        // Mark as read immediately if window is focused
        if (document.hasFocus()) {
           markAsRead(message.conversationId);
        }
      }

      // Update conversations list (last message, unread count)
      const convIndex = state.conversations.findIndex(c => c.id === message.conversationId);
      if (convIndex !== -1) {
        state.conversations[convIndex].lastMessageAt = message.sentAt;
        if (message.conversationId !== state.activeConversationId || !document.hasFocus()) {
          state.conversations[convIndex].unreadCount++;
        }
      }
    });

    state.connection.on("UserTyping", (senderId, conversationId) => {
      if (!state.typingUsers[conversationId]) {
        state.typingUsers[conversationId] = true;
      }
      // Clear after 3 seconds
      setTimeout(() => {
        state.typingUsers[conversationId] = false;
      }, 3000);
    });

    state.connection.on("MessagesRead", (conversationId, readByUserId) => {
      if (state.activeConversationId === conversationId) {
        state.messages.forEach(m => {
          if (m.senderId !== readByUserId) {
            m.isRead = true;
          }
        });
      }
    });

    try {
      await state.connection.start();
    } catch (err) {
      console.error("SignalR Connection Error: ", err);
    }
  }

  async function loadConversations() {
    try {
      const res = await fetch("/api/Chat/conversations", { credentials: "include" });
      if (res.ok) {
        state.conversations = await res.json();
      }
    } catch (e) {
      console.error(e);
    }
  }

  async function loadMessages(conversationId) {
    state.activeConversationId = conversationId;
    state.messages = [];
    try {
      const res = await fetch(`/api/Chat/${conversationId}/messages`, { credentials: "include" });
      if (res.ok) {
        state.messages = await res.json();
        const convIndex = state.conversations.findIndex(c => c.id === conversationId);
        if (convIndex !== -1 && state.conversations[convIndex].unreadCount > 0) {
           markAsRead(conversationId);
           state.conversations[convIndex].unreadCount = 0;
        }
      }
    } catch (e) {
      console.error(e);
    }
  }

  async function sendMessage(conversationId, targetUserId, content) {
    if (isConnected.value) {
      try {
        await state.connection.invoke("SendMessage", conversationId, targetUserId, content);
      } catch (e) {
        console.error(e);
      }
    }
  }

  function sendTyping(conversationId, targetUserId) {
    if (isConnected.value) {
      state.connection.invoke("Typing", targetUserId, conversationId).catch(console.error);
    }
  }

  function markAsRead(conversationId) {
    if (isConnected.value) {
       state.connection.invoke("MarkAsRead", conversationId).catch(console.error);
    }
  }

  async function getOrCreateConversation(targetUserId) {
    try {
       const res = await fetch(`/api/Chat/get-or-create/${targetUserId}`, { 
           method: "POST", 
           credentials: "include" 
       });
       if (res.ok) {
           const data = await res.json();
           return data.conversationId;
       }
    } catch (e) {
       console.error(e);
    }
    return null;
  }

  return {
    state,
    isConnected,
    connect,
    loadConversations,
    loadMessages,
    sendMessage,
    sendTyping,
    markAsRead,
    getOrCreateConversation
  };
}
