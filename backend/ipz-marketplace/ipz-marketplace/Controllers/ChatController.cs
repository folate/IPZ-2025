using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ipz_marketplace.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly MarketplaceDbContext _context;
    private readonly UserManager<User> _userManager;

    public ChatController(MarketplaceDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var conversations = await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .Select(c => new
            {
                id = c.Id,
                otherUserId = c.User1Id == userId ? c.User2Id : c.User1Id,
                otherUserName = c.User1Id == userId 
                    ? c.User2.FirstName + " " + c.User2.LastName 
                    : c.User1.FirstName + " " + c.User1.LastName,
                lastMessageAt = c.LastMessageAt,
                unreadCount = c.User1Id == userId ? c.UnreadCountUser1 : c.UnreadCountUser2
            })
            .ToListAsync();

        return Ok(conversations);
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<IActionResult> GetMessages(int conversationId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null) return NotFound("Conversation not found");

        if (conversation.User1Id != userId && conversation.User2Id != userId)
            return Forbid("You are not part of this conversation");

        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .Select(m => new
            {
                id = m.Id,
                conversationId = m.ConversationId,
                senderId = m.SenderId,
                senderName = m.Sender.FirstName + " " + m.Sender.LastName,
                content = m.Content,
                sentAt = m.SentAt,
                isRead = m.IsRead
            })
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost("get-or-create/{targetUserId}")]
    public async Task<IActionResult> GetOrCreateConversation(string targetUserId)
    {
        var currentUserId = _userManager.GetUserId(User);
        if (currentUserId == null) return Unauthorized();

        if (currentUserId == targetUserId)
            return BadRequest("Cannot create a conversation with yourself");

        var targetUser = await _userManager.FindByIdAsync(targetUserId);
        if (targetUser == null) return NotFound("Target user not found");

        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c =>
                (c.User1Id == currentUserId && c.User2Id == targetUserId) ||
                (c.User1Id == targetUserId && c.User2Id == currentUserId));

        if (conversation == null)
        {
            conversation = new Conversation
            {
                User1Id = currentUserId,
                User2Id = targetUserId,
                LastMessageAt = DateTime.UtcNow,
                UnreadCountUser1 = 0,
                UnreadCountUser2 = 0
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
        }

        return Ok(new { conversationId = conversation.Id });
    }
}
