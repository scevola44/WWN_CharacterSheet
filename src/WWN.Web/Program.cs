using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using WWN.Application.Email;
using WWN.Application.Services;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Email;
using WWN.Infrastructure.Identity;
using WWN.Infrastructure.Persistence;
using WWN.Infrastructure.Repositories;
using WWN.Web.Endpoints;
using WWN.Web.Middleware;
using WWN.Web.Services;

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
        options.SignIn.RequireConfirmedEmail = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<WwnDbContext>()
    .AddDefaultTokenProviders();

    // Email + URL config for confirmation/reset links
    builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Email:Smtp"));
    builder.Services.Configure<AppUrlsOptions>(builder.Configuration.GetSection("AppUrls"));
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

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

    builder.Services.AddAuthorization(options =>
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")));

    // App info service for branch tracking
    var appInfo = new AppInfoService(builder.Configuration);
    builder.Services.AddSingleton(appInfo);

    // Services
    builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
    builder.Services.AddScoped<ISpellRepository, SpellRepository>();
    builder.Services.AddScoped<IArtRepository, ArtRepository>();
    builder.Services.AddScoped<IArtSourceRepository, ArtSourceRepository>();
    builder.Services.AddScoped<IFocusDefinitionRepository, FocusDefinitionRepository>();
    builder.Services.AddScoped<IClassAbilityRepository, ClassAbilityRepository>();
    builder.Services.AddScoped<CharacterDetailMapper>();
    builder.Services.AddScoped<CharacterIdentityService>();
    builder.Services.AddScoped<CharacterFocusService>();
    builder.Services.AddScoped<CharacterInventoryService>();
    builder.Services.AddScoped<SpellService>();
    builder.Services.AddScoped<ArtService>();
    builder.Services.AddScoped<ArtSourceService>();
    builder.Services.AddScoped<LookupsService>();
    builder.Services.AddScoped<CharacterSpellService>();
    builder.Services.AddScoped<CharacterArtService>();
    builder.Services.AddScoped<FocusDefinitionService>();
    builder.Services.AddScoped<FocusDefinitionSeeder>();
    builder.Services.AddScoped<SpellDefinitionSeeder>();
    builder.Services.AddScoped<ArtDefinitionSeeder>(provider =>
        new ArtDefinitionSeeder(
            provider.GetRequiredService<IArtRepository>(),
            provider.GetRequiredService<IArtSourceRepository>()
        ));

    builder.Services.AddScoped<ClassAbilitySeeder>();
    builder.Services.AddScoped<AdminRoleSeeder>();
    builder.Services.AddSingleton<CharacterSheetCalculator>();
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<ValidationFilter>();

    // DataProtection — persist keys to the mounted volume so they survive container restarts
    var dpKeysPath = builder.Configuration["DataProtection:KeysPath"]
        ?? Path.Combine(builder.Environment.ContentRootPath, "keys");
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(dpKeysPath));

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // CORS for React dev server
    builder.Services.AddCors(opt => opt.AddDefaultPolicy(p =>
        p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

    var app = builder.Build();

    Log.Information("Application started - Branch: {Branch}, Environment: {Environment}", appInfo.Branch, appInfo.Environment);

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

        // Seed default WWN arts from the Free Edition if the table is empty.
        var artSeeder = scope.ServiceProvider.GetRequiredService<ArtDefinitionSeeder>();
        await artSeeder.SeedIfEmptyAsync();

        // Seed default WWN class abilities from the Free Edition if the table is empty.
        var abilitySeeder = scope.ServiceProvider.GetRequiredService<ClassAbilitySeeder>();
        await abilitySeeder.SeedIfEmptyAsync();

        // Create the Admin role and assign it to any configured AdminEmails.
        var adminSeeder = scope.ServiceProvider.GetRequiredService<AdminRoleSeeder>();
        await adminSeeder.SeedAsync();
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

    var root = app.MapGroup("").AddEndpointFilter<ValidationFilter>();
    root.MapAuthEndpoints();
    root.MapCharacterEndpoints();
    root.MapSpellEndpoints();
    root.MapArtEndpoints();
    root.MapFocusDefinitionEndpoints();
    root.MapDiagnosticsEndpoints();
    root.MapLookupsEndpoints();
    root.MapArtSourceEndpoints();
    root.MapAdminEndpoints();

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
