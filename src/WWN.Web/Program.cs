using Microsoft.EntityFrameworkCore;
using WWN.Application.Services;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;
using WWN.Infrastructure.Repositories;
using WWN.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<WwnDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=wwn_characters.db"));

// Services
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ISpellRepository, SpellRepository>();
builder.Services.AddScoped<CharacterService>();
builder.Services.AddScoped<SpellService>();
builder.Services.AddScoped<CharacterSpellService>();
builder.Services.AddScoped<CharacterSheetCalculator>();

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
}

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
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { } // For integration tests
