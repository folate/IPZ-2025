using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ipz_marketplace.Entities;

public class Conversation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string User1Id { get; set; } = null!;

    [Required]
    public string User2Id { get; set; } = null!;

    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

    public int UnreadCountUser1 { get; set; } = 0;
    public int UnreadCountUser2 { get; set; } = 0;

    [ForeignKey("User1Id")]
    public virtual User User1 { get; set; } = null!;

    [ForeignKey("User2Id")]
    public virtual User User2 { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
