using ipz_marketplace.Controllers;
using ipz_marketplace.Data;
using ipz_marketplace.Entities;
using ipz_marketplace.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace ipz_marketplace;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                             .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
                             .AddEnvironmentVariables();

        var keysDirectory = new DirectoryInfo("/root/.aspnet/DataProtection-Keys");
        // var keysDirectory = new DirectoryInfo("../../keys/DataProtection-Keys");

        if (!keysDirectory.Exists)
        {
            keysDirectory.Create();
        }
        
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(keysDirectory)
            .SetApplicationName("ipz-marketplace");

        builder.Services.AddScoped<AuthService>();

        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://localhost")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        builder.Services.AddSingleton<EmailService>();

        builder.Services.AddSingleton<OrderService>();

        builder.Services.AddDbContext<MarketplaceDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddDataProtection();
        builder.Services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MarketplaceDbContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        })
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.Cookie.Name = ".AspNetCore.Identity.Application";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });

        var app = builder.Build();

        // Seed database
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try 
            {
                var context = services.GetRequiredService<MarketplaceDbContext>();
                
                context.Database.EnsureDeleted();
        
                context.Database.Migrate();

                await DatabaseSeeder.SeedAsync(services, app.Environment);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration or seeding.");
            }
        }


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "My API v1");
                options.RoutePrefix = "api/swagger";
            });
        }

        app.UseStaticFiles();

        var photosPath = Path.Combine(Directory.GetCurrentDirectory(), "photos");
        Console.WriteLine($"Photos path: {photosPath}");
        if (!Directory.Exists(photosPath))
        {
            Directory.CreateDirectory(photosPath);
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(photosPath),
            RequestPath = "/photos"
        });

        app.UseRouting();

        app.UseCors();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}