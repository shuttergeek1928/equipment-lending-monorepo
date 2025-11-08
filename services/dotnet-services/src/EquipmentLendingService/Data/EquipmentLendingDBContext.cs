using System;
using System.Collections.Generic;
using EquipmentLendingDotnetServices.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentLendingDotnetServices.Data;

public partial class EquipmentLendingDBContext : DbContext
{
    public EquipmentLendingDBContext()
    {
    }

    public EquipmentLendingDBContext(DbContextOptions<EquipmentLendingDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BorrowingsAndReturns> BorrowingsAndReturns { get; set; }

    public virtual DbSet<Equipment> Equipments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Usertype> Usertypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=equipmentlendingdatabase;Username=postgres;Password=admin;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BorrowingsAndReturns>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("borrowingsandreturns_pkey");

            entity.ToTable("borrowingsandreturns");

            entity.HasIndex(e => e.RequestId, "borrowingsandreturns_requestid_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EquipmentId).HasColumnName("equipmentid");
            entity.Property(e => e.IsApproved)
                .HasDefaultValue(false)
                .HasColumnName("isapproved");
            entity.Property(e => e.IsReturned)
                .HasDefaultValue(false)
                .HasColumnName("isreturned");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");
            entity.Property(e => e.RequestedOn).HasColumnName("requestedon");
            entity.Property(e => e.RequestedQuantity).HasColumnName("requestedquantity");
            entity.Property(e => e.RequesterId).HasColumnName("requesterid");
            entity.Property(e => e.RequestId)
                .ValueGeneratedOnAdd()
                .HasColumnName("requestid");
            entity.Property(e => e.ReturnDueDate).HasColumnName("returnduedate");
            entity.Property(e => e.ReturnedOn).HasColumnName("returnedon");

            entity.HasOne(d => d.Equipment).WithMany(p => p.BorrowingsAndReturns)
                .HasPrincipalKey(p => p.EquipmentId)
                .HasForeignKey(d => d.EquipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bnr_equipment");

            entity.HasOne(d => d.Requester).WithMany(p => p.BorrowingsAndReturns)
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(d => d.RequesterId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_bnr_requester");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("equipments_pkey");

            entity.ToTable("equipments");

            entity.HasIndex(e => e.EquipmentId, "equipments_equipmentid_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AddedOn)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("addedon");
            entity.Property(e => e.AvailableQuantity)
                .HasDefaultValue(1)
                .HasColumnName("availablequantity");
            entity.Property(e => e.Catgory)
                .HasMaxLength(255)
                .HasColumnName("catgory");
            entity.Property(e => e.EquipmentCondition)
                .HasMaxLength(255)
                .HasColumnName("equipmentcondition");
            entity.Property(e => e.EquipmentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("equipmentid");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(255)
                .HasColumnName("equipmentname");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("isavailable");
            entity.Property(e => e.TotalQuantity)
                .HasDefaultValue(1)
                .HasColumnName("totalquantity");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.UserId, "users_userid_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdat");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(false)
                .HasColumnName("isactive");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.LastAccessedAt).HasColumnName("lastaccessedat");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("passwordhash");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(255)
                .HasColumnName("passwordsalt");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedat");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("username");
            entity.Property(e => e.UserType).HasColumnName("usertype");

            entity.HasOne(d => d.UserTypeNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_usertype_fkey");
        });

        modelBuilder.Entity<Usertype>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("usertypes_pkey");

            entity.ToTable("usertypes");

            entity.HasIndex(e => e.TypeValue, "usertypes_typevalue_key").IsUnique();

            entity.Property(e => e.TypeId)
                .ValueGeneratedNever()
                .HasColumnName("typeid");
            entity.Property(e => e.TypeValue)
                .HasMaxLength(100)
                .HasColumnName("typevalue");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
