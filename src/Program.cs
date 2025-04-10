
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalAPITemplate.Infrastructure.Data;
using MinimalAPITemplate.Infrastructure.Extensions;
using MinimalAPITemplate.Infrastructure.Identity;
using Scalar.AspNetCore;

namespace MinimalAPITemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOpenApi();

            builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme)
                .AddBearerToken(IdentityConstants.BearerScheme);

            builder.Services.AddAuthorization();

            builder.Services
                .AddIdentityCore<ApplicationUser>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddApiEndpoints();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable("APP_DB_CONNECTION")));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options
                        .WithTitle("Minimal API")
                        .WithTheme(ScalarTheme.Mars)
                        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            }

            app.ApplyMigrations();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    SeedExtensions.Initialize(services).GetAwaiter().GetResult(); 
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");

            app.MapIdentityApi<ApplicationUser>().WithTags("Identity");

            app.Run();
        }
    }
}
