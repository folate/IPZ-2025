using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ipz_marketplace;

public class MarketplaceDbContext : IdentityDbContext<User>
{
    public MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options) : base(options) { }
    
    public DbSet<SellerAd> SellerAds { get; set; }
    public DbSet<Gigs> Gigs { get; set; }
    public DbSet<Buyer> Buyers { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BuyerAd> BuyerAds { get; set; }
    public DbSet<BuyerAdOffer> BuyerAdOffers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Revision> Revisions { get; set; }
    public DbSet<RevisionFile> RevisionFiles { get; set; }
    public DbSet<AdPhoto> Photos { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        builder.Entity<User>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(256);
        });

        builder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        builder.Entity<SellerAd>()
            .HasOne(ad => ad.Freelancer)
            .WithMany()
            .HasForeignKey(ad => ad.FreelancerId);

        builder.Entity<Gigs>(entity =>
        {
            entity.HasOne(g => g.SellerAd)
                  .WithMany(a => a.Gigs)
                  .HasForeignKey(g => g.SellerAdId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(g => g.Price)
                  .HasColumnType("numeric(18,2)");
        });

        builder.Entity<AdPhoto>(entity =>
        {
            entity.HasOne(p => p.SellerAd)
                  .WithMany(a => a.Photos)
                  .HasForeignKey(p => p.SellerAdId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Buyer>()
            .HasOne(b => b.User)
            .WithOne(u => u.Buyer)
            .HasForeignKey<Buyer>(b => b.UserId);
        builder.Entity<Buyer>()
            .HasIndex(b => b.UserId)
            .IsUnique();

        builder.Entity<Seller>()
            .HasOne(s => s.User)
            .WithOne(u => u.Seller)
            .HasForeignKey<Seller>(s => s.UserId);
        builder.Entity<Seller>()
            .HasIndex(b => b.UserId)
            .IsUnique();

        builder.Entity<Order>()
            .HasOne(o => o.Gigs)
            .WithMany()
            .HasForeignKey(o => o.GigsId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Order>()
            .HasOne(o => o.Seller)
            .WithMany()
            .HasForeignKey(o => o.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Order>()
            .HasOne(o => o.Buyer)
            .WithMany()
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RevisionFile>(entity =>
        {
            entity.HasOne(f => f.Revision)
                  .WithMany(r => r.Files)
                  .HasForeignKey(f => f.RevisionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Review>(entity =>
        {
            entity.HasOne(r => r.Seller)
                  .WithMany()
                  .HasForeignKey(r => r.SellerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Buyer)
                  .WithMany()
                  .HasForeignKey(r => r.BuyerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Conversation>(entity =>
        {
            entity.HasOne(c => c.User1)
                  .WithMany()
                  .HasForeignKey(c => c.User1Id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.User2)
                  .WithMany()
                  .HasForeignKey(c => c.User2Id)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Message>(entity =>
        {
            entity.HasOne(m => m.Conversation)
                  .WithMany(c => c.Messages)
                  .HasForeignKey(m => m.ConversationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Sender)
                  .WithMany()
                  .HasForeignKey(m => m.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
