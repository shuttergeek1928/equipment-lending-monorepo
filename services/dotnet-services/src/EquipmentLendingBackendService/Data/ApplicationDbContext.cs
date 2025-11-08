using System;
using System.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using EquipmentLendingDotnetServices.Models;
using EquipmentLendingBackendService.Models;

namespace EquipmentLendingDotnetServices.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Equipment> Equipments { get; set; } = null!;
    public DbSet<BorrowingsAndReturns> BorrowingsAndReturns { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Usertype> Usertypes { get; set; } = null!;
    public DbSet<BorrowNotification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Equipment
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId);
            entity.Property(e => e.EquipmentId).ValueGeneratedOnAdd();

            entity.Property(e => e.EquipmentName).IsRequired();
            // keep the property name as in the model ("Catgory") – map column type if desired
            entity.Property(e => e.EquipmentCondition).HasDefaultValue("Good");
            entity.Property(e => e.TotalQuantity).HasDefaultValue(1);
            entity.Property(e => e.AvailableQuantity).HasDefaultValue(1);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

            // DateOnly -> date mapping for PostgreSQL
            entity.Property(e => e.AddedOn)
                  .HasColumnType("date")
                  .HasConversion(
                      v => v.ToDateTime(new TimeOnly(0, 0)),
                      v => DateOnly.FromDateTime(v))
                  .HasDefaultValueSql("CURRENT_DATE");

            entity.HasMany(e => e.BorrowingsAndReturns)
                  .WithOne(b => b.Equipment)
                  .HasForeignKey(b => b.EquipmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserId).ValueGeneratedOnAdd();

            entity.Property(u => u.UserName).IsRequired();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.PasswordSalt).IsRequired();

            entity.Property(u => u.IsActive).HasDefaultValue(true);
            entity.Property(u => u.IsDeleted).HasDefaultValue(false);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasMany(u => u.BorrowingsAndReturns)
                  .WithOne(b => b.Requester)
                  .HasForeignKey(b => b.RequesterId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.UserTypeNavigation)
                  .WithMany(t => t.Users)
                  .HasForeignKey(u => u.UserType)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Usertype
        modelBuilder.Entity<Usertype>(entity =>
        {
            entity.HasKey(t => t.TypeId);
            entity.Property(t => t.TypeValue).IsRequired();
        });

        // BorrowingsAndReturns
        modelBuilder.Entity<BorrowingsAndReturns>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Id).ValueGeneratedOnAdd();

            // store timestamps in Postgres (with time zone recommended if you want uniform timezone handling)
            entity.Property(b => b.RequestedOn).HasColumnType("timestamp with time zone");
            entity.Property(b => b.ReturnDueDate).HasColumnType("timestamp with time zone");
            entity.Property(b => b.ReturnedOn).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<BorrowNotification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Id).ValueGeneratedOnAdd();
            entity.Property(n => n.NotifiedAt).HasColumnType("timestamp with time zone");
            entity.HasOne(n => n.BorrowRequest)
                  .WithMany(b => b.Notifications)
                  .HasForeignKey(n => n.Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

/// <summary>
/// Design-time factory so EF tools can create the DbContext for migrations.
/// Reads connection string from appsettings.json (key: ConnectionStrings:DefaultConnection),
/// falls back to env var CONNECTION_STRING, or to a hardcoded default.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Build configuration from appsettings and environment
        var basePath = Directory.GetCurrentDirectory();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        // Try config connection string, then env var, then hardcoded fallback
        var conn = config.GetConnectionString("DefaultConnection")
                   ?? Environment.GetEnvironmentVariable("CONNECTION_STRING")
                   ?? "Host=localhost;Port=5432;Database=equipmentlendingdatabase;Username=postgres;Password=admin";

        builder.UseNpgsql(conn);

        return new ApplicationDbContext(builder.Options);
    }
}