using Microsoft.EntityFrameworkCore;
using Serilog;
using WWN.Application.Services;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;
using WWN.Infrastructure.Repositories;
using WWN.Web.Endpoints;
using WWN.Web.Middleware;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((ctx, services, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .ReadFrom.Services(services));

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
    builder.Services.AddScoped<SpellDefinitionSeeder>();
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
        var dbContext = scope.ServiceProvider.GetRequiredService<WwnDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        // Seed default WWN foci from the Free Edition if the table is empty.
        var focusSeeder = scope.ServiceProvider.GetRequiredService<FocusDefinitionSeeder>();
        await focusSeeder.SeedIfEmptyAsync();

        // Seed default WWN spells from the Free Edition if the table is empty.
        var spellSeeder = scope.ServiceProvider.GetRequiredService<SpellDefinitionSeeder>();
        await spellSeeder.SeedIfEmptyAsync();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseCors();
    app.UseSerilogRequestLogging();

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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { } // For integration tests
