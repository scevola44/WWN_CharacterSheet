using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using WWN.Application.Services;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Identity;
using WWN.Infrastructure.Persistence;
using WWN.Infrastructure.Repositories;
using WWN.Web.Endpoints;
using WWN.Web.Middleware;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.re{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
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

    // Identity — use AddIdentityCore (not AddIdentity) to avoid cookie auth overriding JWT
    builder.Services.AddIdentityCore<AppUser>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<WwnDbContext>();

    // JWT Authentication
    var jwtKey = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("Jwt:Key is not configured.");
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization();

    // Services
    builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
    builder.Services.AddScoped<ISpellRepository, SpellRepository>();
    builder.Services.AddScoped<IFocusDefinitionRepository, FocusDefinitionRepository>();
    builder.Services.AddScoped<IClassAbilityRepository, ClassAbilityRepository>();
    builder.Services.AddScoped<CharacterService>();
    builder.Services.AddScoped<SpellService>();
    builder.Services.AddScoped<CharacterSpellService>();
    builder.Services.AddScoped<FocusDefinitionService>();
    builder.Services.AddScoped<FocusDefinitionSeeder>();
    builder.Services.AddScoped<SpellDefinitionSeeder>();
    builder.Services.AddScoped<ClassAbilitySeeder>();
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
        await dbContext.Database.MigrateAsync();

        // Seed default WWN foci from the Free Edition if the table is empty.
        var focusSeeder = scope.ServiceProvider.GetRequiredService<FocusDefinitionSeeder>();
        await focusSeeder.SeedIfEmptyAsync();

        // Seed default WWN spells from the Free Edition if the table is empty.
        var spellSeeder = scope.ServiceProvider.GetRequiredService<SpellDefinitionSeeder>();
        await spellSeeder.SeedIfEmptyAsync();

        // Seed default WWN class abilities from the Free Edition if the table is empty.
        var abilitySeeder = scope.ServiceProvider.GetRequiredService<ClassAbilitySeeder>();
        await abilitySeeder.SeedIfEmptyAsync();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseCors();
    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapAuthEndpoints();
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
