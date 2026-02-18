using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

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

        builder.Entity<Buyer>()
            .HasOne(b => b.User)
            .WithOne(u => u.Buyer)
            .HasForeignKey<Buyer>(b => b.UserId);

        builder.Entity<Seller>()
            .HasOne(s => s.User)
            .WithOne(u => u.Seller)
            .HasForeignKey<Seller>(s => s.UserId);
    }
}
