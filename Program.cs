using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<PortfolioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PortfolioContext>();
    context.Database.Migrate();

    var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "projects.json");

    Console.WriteLine($"JSON path: {jsonPath}");
    Console.WriteLine($"File exists: {File.Exists(jsonPath)}");

    if (File.Exists(jsonPath))
    {
        var json = File.ReadAllText(jsonPath);

        var projects = JsonSerializer.Deserialize<List<ProjectItem>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        Console.WriteLine($"Deserialized count: {projects?.Count ?? 0}");

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

            Console.WriteLine($"DB count after save: {context.Projects.Count()}");
        }
    }
}

app.Run();