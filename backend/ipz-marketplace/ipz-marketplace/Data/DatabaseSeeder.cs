using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Identity;

namespace ipz_marketplace.Data;

public class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IWebHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            return;
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = serviceProvider.GetRequiredService<MarketplaceDbContext>();

        // Sprawdź czy baza jest pusta
        if (userManager.Users.Any())
        {
            return;
        }

        // Utwórz rolę Admin jeśli nie istnieje
        var Roles = new[] { "Admin", "Buyer", "Seller" };

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Utwórz użytkownika admin
        var adminUser = new User
        {
            UserName = "admin",
            Email = "admin@marketplace.com",
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "Administrator",
            CreateDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        var tab = new[] { "buyer", "freelancer" };

        foreach (var name in tab) {
            var user = new User
            {
                UserName = name,
                Email = "user@marketplace.com",
                EmailConfirmed = true,
                FirstName = "uesr",
                LastName = "using",
                CreateDate = DateTime.UtcNow
            };

            if ((await userManager.CreateAsync(user, "Tuba123!")).Succeeded)
            {
                if (user.UserName == "freelancer")
                {
                    await userManager.AddToRoleAsync(user, "Seller");
                    var sellerAd = new SellerAd
                    {
                        Id = 1,
                        Title = "Test Seller Ad",
                        Description = "Description of testing sellerAd.",
                        CreateDate = DateTime.UtcNow,
                        Freelancer = user,
                        FreelancerId = user.Id
                    };

                    var gig = new Gigs
                    {
                        Id = 1,
                        TierName = "Sample Gig",
                        TierDescription = "Description of tier.",
                        Price = 100,
                        SellerAdId = sellerAd.Id,
                        SellerAd = sellerAd
                    };
                    context.SellerAds.Add(sellerAd);
                    context.Gigs.Add(gig);

                    var gig2 = new Gigs
                    {
                        Id = 2,
                        TierName = "Sample Gig 2",
                        TierDescription = "Description of tier.",
                        Price = 200,
                        SellerAdId = sellerAd.Id,
                        SellerAd = sellerAd
                    };
                    context.Gigs.Add(gig2);
                }
                else
                {
                    await userManager.AddToRoleAsync(user, "Buyer");
                    var userBuyer = new Buyer
                    {
                        UserId = user.Id,
                        ShippingAddress = "456 User Street",
                        BillingAddress = "456 User Street",
                        TotalOrders = 0,
                        JoinedDate = DateTime.UtcNow,
                        LastOrderDate = null,
                        PreferredPaymentMethod = "PayPal"
                    };
                    context.Buyers.Add(userBuyer);
                }
            }

        }

        var adminBuyer = new Buyer
        {
            UserId = adminUser.Id,
            ShippingAddress = "123 Admin Street",
            BillingAddress = "123 Admin Street",
            TotalOrders = 0,
            JoinedDate = DateTime.UtcNow,
            LastOrderDate = null,
            PreferredPaymentMethod = "Credit Card"
        };

        context.Buyers.Add(adminBuyer);

        var categories = new string[]
        {
            "Web Development",
            "Graphic Design",
            "Content Writing",
            "Digital Marketing",
            "Video Editing"
        };

        foreach (var categoryName in categories)
        {
            var category = new ipz_marketplace.Entities.Category
            {
                Name =  categoryName
            };
            context.Categories.Add(category);
        }

        await context.SaveChangesAsync();
    }
}
