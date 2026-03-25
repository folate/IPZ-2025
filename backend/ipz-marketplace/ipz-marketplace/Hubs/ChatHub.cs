using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ipz_marketplace.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly MarketplaceDbContext _context;
    private readonly UserManager<User> _userManager;

    public ChatHub(MarketplaceDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SendMessage(int conversationId, string targetUserId, string content)
    {
        var senderId = Context.UserIdentifier;
        if (senderId == null) return;

        // Verify conversation belongs to sender and target
        var conversation = await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null) return;

        // Security check: Only participants can send messages
        if (conversation.User1Id != senderId && conversation.User2Id != senderId)
            return;

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Messages.Add(message);

        conversation.LastMessageAt = DateTime.UtcNow;
        
        if (conversation.User1Id == targetUserId)
        {
            conversation.UnreadCountUser1++;
        }
        else if (conversation.User2Id == targetUserId)
        {
            conversation.UnreadCountUser2++;
        }

        await _context.SaveChangesAsync();

        var senderUser = await _userManager.FindByIdAsync(senderId);

        var messageDto = new
        {
            id = message.Id,
            conversationId = message.ConversationId,
            senderId = message.SenderId,
            senderName = senderUser?.FirstName + " " + senderUser?.LastName,
            content = message.Content,
            sentAt = message.SentAt,
            isRead = message.IsRead
        };

        // Send to target user
        await Clients.User(targetUserId).SendAsync("ReceiveMessage", messageDto);
        // Send back to sender so their UI can add it safely if they have multiple devices
        await Clients.User(senderId).SendAsync("ReceiveMessage", messageDto);
    }

    public async Task Typing(string targetUserId, int conversationId)
    {
        var senderId = Context.UserIdentifier;
        if (senderId == null) return;

        await Clients.User(targetUserId).SendAsync("UserTyping", senderId, conversationId);
    }

    public async Task MarkAsRead(int conversationId)
    {
        var currentUserId = Context.UserIdentifier;
        if (currentUserId == null) return;

        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null) return;

        if (conversation.User1Id == currentUserId)
        {
            conversation.UnreadCountUser1 = 0;
        }
        else if (conversation.User2Id == currentUserId)
        {
            conversation.UnreadCountUser2 = 0;
        }

        var unreadMessages = await _context.Messages
            .Where(m => m.ConversationId == conversationId && m.SenderId != currentUserId && !m.IsRead)
            .ToListAsync();

        foreach (var msg in unreadMessages)
        {
            msg.IsRead = true;
        }

        await _context.SaveChangesAsync();

        var otherUserId = conversation.User1Id == currentUserId ? conversation.User2Id : conversation.User1Id;
        
        // Notify the sender that their messages were read
        await Clients.User(otherUserId).SendAsync("MessagesRead", conversationId, currentUserId);
    }
}
