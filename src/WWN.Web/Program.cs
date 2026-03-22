using Microsoft.EntityFrameworkCore;
using WWN.Application.Services;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;
using WWN.Infrastructure.Repositories;
using WWN.Web.Endpoints;
using WWN.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<WwnDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=wwn_characters.db"));

// Services
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ISpellRepository, SpellRepository>();
builder.Services.AddScoped<IFocusDefinitionRepository, FocusDefinitionRepository>();
builder.Services.AddScoped<CharacterService>();
builder.Services.AddScoped<SpellService>();
builder.Services.AddScoped<CharacterSpellService>();
builder.Services.AddScoped<FocusDefinitionService>();
builder.Services.AddScoped<FocusDefinitionSeeder>();
builder.Services.AddSingleton<CharacterSheetCalculator>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for React dev server
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Auto-migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WwnDbContext>();
    await db.Database.EnsureCreatedAsync();

    // Create FocusDefinitions table if it doesn't exist (handles databases
    // created before this feature was added).
    await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS ""FocusDefinitions"" (
            ""Id""                 TEXT    NOT NULL CONSTRAINT ""PK_FocusDefinitions"" PRIMARY KEY,
            ""Name""               TEXT    NOT NULL,
            ""Description""        TEXT,
            ""Level1Description""  TEXT    NOT NULL,
            ""Level2Description""  TEXT,
            ""HasLevel2""          INTEGER NOT NULL DEFAULT 0,
            ""CanTakeMultipleTimes"" INTEGER NOT NULL DEFAULT 0
        )");

    // Seed default WWN foci from the Free Edition if the table is empty.
    var seeder = scope.ServiceProvider.GetRequiredService<FocusDefinitionSeeder>();
    await seeder.SeedIfEmptyAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapCharacterEndpoints();
app.MapSpellEndpoints();
app.MapFocusDefinitionEndpoints();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { } // For integration tests
