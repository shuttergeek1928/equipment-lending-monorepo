using EquipmentLendingBackendService.Security;
using EquipmentLendingBackendService.Services;
using EquipmentLendingDotnetServices.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext (reads connection string from appsettings: "ConnectionStrings:Default" or env var CONNECTION_STRING)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string not configured. Set ConnectionStrings:Default in appsettings.json or set CONNECTION_STRING env var.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddControllers();

//Add email dependencies
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//builder.Services.AddHostedService<OverdueNotificationService>();
builder.Services.AddHostedService<TestEmailBackgroundService>();
builder.Services.AddScoped<INotificationSender, SmtpNotificationSender>();

//Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//Add http client
builder.Services.AddHttpClient();

// Load secret from config or env var
var configuration = builder.Configuration;
var secret = configuration["Jwt:Secret"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_JWT_SECRET");
if (string.IsNullOrEmpty(secret))
    throw new Exception("JWT secret not set. Set Jwt:Secret in configuration or ASPNETCORE_JWT_SECRET env var.");

var issuer = configuration["Jwt:Issuer"] ?? "http://localhost:8081"; // change to match Java 'iss'
var audience = configuration["Jwt:Audience"] ?? "your-api-audience";

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

//Add the JWT Bearer authentication scehme
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false; // true in prod (use https)
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(60),
            RoleClaimType = "roles",
            NameClaimType = "sub"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation, RolePrefixStripper>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}' (without quotes). Example: \"Bearer eyJ...\"",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    // Make swagger UI send the token on each request
    var securityRequirement = new OpenApiSecurityRequirement
        {
            { securityScheme, Array.Empty<string>() }
        };
    c.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

// Seed database (runs migrations and inserts initial data)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        // DataSeeder.SeedAsync will apply pending migrations and insert seed data.
        await DataSeeder.SeedAsync(db);
        logger.LogInformation("Database seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();