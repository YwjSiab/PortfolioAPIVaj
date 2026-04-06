using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Render port binding
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers();

builder.Services.AddDbContext<PortfolioContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://ntc-portfolio-vaj.infinityfreeapp.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PortfolioContext>();
    context.Database.Migrate();

    var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "projects.json");

    if (File.Exists(jsonPath))
    {
        var json = File.ReadAllText(jsonPath);

        var projects = JsonSerializer.Deserialize<List<ProjectItem>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (projects is not null && projects.Any())
        {
            context.ChangeTracker.Clear();
            context.Projects.RemoveRange(context.Projects);
            context.SaveChanges();

            var distinctProjects = projects
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .ToList();

            context.Projects.AddRange(distinctProjects);
            context.SaveChanges();
        }
    }
}

app.Run();
