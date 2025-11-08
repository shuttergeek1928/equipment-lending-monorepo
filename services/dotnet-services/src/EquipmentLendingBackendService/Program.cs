using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EquipmentLendingDotnetServices.Data;
using EquipmentLendingBackendService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext (reads connection string from appsettings: "ConnectionStrings:Default" or env var CONNECTION_STRING)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string not configured. Set ConnectionStrings:Default in appsettings.json or set CONNECTION_STRING env var.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<OverdueNotificationService>();
builder.Services.AddScoped<INotificationSender, SmtpNotificationSender>();

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();