using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EquipmentLendingDotnetServices.Models;
using System.Collections.Generic;

namespace EquipmentLendingDotnetServices.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Apply pending migrations (safe for dev)
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            // Use a transaction so partial seeds don't leave the DB in an inconsistent state
            await using var tx = await context.Database.BeginTransactionAsync();

            // 1) Seed user types (upsert by TypeValue)
            var userTypesToEnsure = new[]
            {
                new Usertype { TypeId = 1, TypeValue = "Admin" },
                new Usertype { TypeId = 2, TypeValue = "Student" },
                new Usertype { TypeId = 3, TypeValue = "Staff" }
            };

            foreach (var ut in userTypesToEnsure)
            {
                var existing = await context.Usertypes
                    .FirstOrDefaultAsync(x => x.TypeValue == ut.TypeValue);

                if (existing == null)
                {
                    await context.Usertypes.AddAsync(ut);
                }
                else
                {
                    // keep existing TypeId if present, but ensure value is normalized
                    if (existing.TypeId != ut.TypeId)
                        existing.TypeId = existing.TypeId; // keep DB id; don't overwrite intentionally
                    existing.TypeValue = ut.TypeValue;
                    context.Usertypes.Update(existing);
                }
            }

            await context.SaveChangesAsync();

            // 2) Seed users (upsert by Email)
            // Passwords are set only for newly created users. Existing users keep their password hash.
            CreatePasswordHash("Admin@123", out var adminHash, out var adminSalt);
            CreatePasswordHash("Student@123", out var studentHash, out var studentSalt);

            var usersToEnsure = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin",
                    Email = "admin@example.com",
                    PasswordHash = adminHash,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "jdoe",
                    Email = "jdoe@example.com",
                    PasswordHash = studentHash,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            foreach (var u in usersToEnsure)
            {
                var existing = await context.Users.FirstOrDefaultAsync(x => x.Email == u.Email);
                if (existing == null)
                {
                    await context.Users.AddAsync(u);
                }
                else
                {
                    // update non-sensitive/profile fields only
                    existing.UserName = u.UserName;
                    existing.IsActive = u.IsActive;
                    existing.IsDeleted = u.IsDeleted;
                    existing.UpdatedAt = DateTime.UtcNow;
                    context.Users.Update(existing);
                }
            }

            await context.SaveChangesAsync();

            // 3) Seed equipment (upsert by EquipmentId OR EquipmentName).
            // When equipment exists we update metadata and adjust quantities so existing borrows are respected.
            var allEquipment = new List<Equipment>
            {
                new Equipment
                {
                    Id = Guid.NewGuid(),
                    EquipmentId = 1,
                    EquipmentName = "Canon DSLR",
                    Catgory = "Camera",
                    EquipmentCondition = "Good",
                    TotalQuantity = 5,
                    AvailableQuantity = 5,
                    IsAvailable = true,
                    AddedOn = DateOnly.FromDateTime(DateTime.UtcNow.Date)
                },

                new Equipment
                {
                    Id = Guid.NewGuid(),
                    EquipmentId = 2,
                    EquipmentName = "Acer Predator",
                    Catgory = "Laptop",
                    EquipmentCondition = "Average",
                    TotalQuantity = 10,
                    AvailableQuantity = 8,
                    IsAvailable = true,
                    AddedOn = DateOnly.FromDateTime(DateTime.UtcNow.Date)
                },

                new Equipment
                {
                    Id = Guid.NewGuid(),
                    EquipmentId = 3,
                    EquipmentName = "Lenovo ThinkPad",
                    Catgory = "Laptop",
                    EquipmentCondition = "Good",
                    TotalQuantity = 3,
                    AvailableQuantity = 3,
                    IsAvailable = true,
                    AddedOn = DateOnly.FromDateTime(DateTime.UtcNow.Date)
                },

                new Equipment
                {
                    Id = Guid.NewGuid(),
                    EquipmentId = 4,
                    EquipmentName = "Epson Projector",
                    Catgory = "AV",
                    EquipmentCondition = "Good",
                    TotalQuantity = 2,
                    AvailableQuantity = 2,
                    IsAvailable = true,
                    AddedOn = DateOnly.FromDateTime(DateTime.UtcNow.Date)
                },

                new Equipment
                {
                    Id = Guid.NewGuid(),
                    EquipmentId = 5,
                    EquipmentName = "Whiteboard",
                    Catgory = "Display",
                    EquipmentCondition = "Excellent",
                    TotalQuantity = 5,
                    AvailableQuantity = 3,
                    IsAvailable = true,
                    AddedOn = DateOnly.FromDateTime(DateTime.UtcNow.Date)
                }
            };

            foreach (var incoming in allEquipment)
            {
                var existing = await context.Equipments
                    .FirstOrDefaultAsync(e => e.EquipmentId == incoming.EquipmentId
                                              || e.EquipmentName == incoming.EquipmentName);

                if (existing == null)
                {
                    await context.Equipments.AddAsync(incoming);
                }
                else
                {
                    // Adjust totals while preserving current borrowed count
                    // borrowed = existing.TotalQuantity - existing.AvailableQuantity
                    var borrowed = Math.Max(0, existing.TotalQuantity - existing.AvailableQuantity);
                    var newTotal = incoming.TotalQuantity;

                    // If the new total increased, increase available by the delta
                    var delta = newTotal - existing.TotalQuantity;
                    if (delta > 0)
                    {
                        existing.AvailableQuantity = existing.AvailableQuantity + delta;
                    }

                    // If the new total decreased below borrowed, clamp available to 0
                    if (newTotal < borrowed)
                    {
                        existing.AvailableQuantity = 0;
                    }
                    else
                    {
                        // ensure available does not exceed total
                        existing.AvailableQuantity = Math.Min(existing.AvailableQuantity, newTotal);
                    }

                    existing.TotalQuantity = newTotal;

                    // Update metadata
                    existing.EquipmentCondition = incoming.EquipmentCondition;
                    existing.Catgory = incoming.Catgory;
                    existing.EquipmentName = incoming.EquipmentName;
                    existing.AddedOn = incoming.AddedOn;
                    existing.IsAvailable = existing.AvailableQuantity > 0;

                    context.Equipments.Update(existing);
                }
            }

            await context.SaveChangesAsync();

            // 4) Seed a borrowing record (approved) — upsert by RequestId
            // Get the student user (by email) and the camera (by name)
            var studentUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "jdoe@example.com")
                              ?? await context.Users.FirstOrDefaultAsync(u => u.UserName == "jdoe");

            var camera = await context.Equipments.FirstOrDefaultAsync(e => e.EquipmentName == "Canon DSLR");

            if (studentUser != null && camera != null)
            {
                var borrowRequestId = 1001;
                var existingBorrow = await context.BorrowingsAndReturns
                    .FirstOrDefaultAsync(b => b.RequestId == borrowRequestId && b.RequesterId == studentUser.UserId);

                if (existingBorrow == null)
                {
                    var borrow = new BorrowingsAndReturns
                    {
                        Id = Guid.NewGuid(),
                        EquipmentId = camera.EquipmentId,
                        RequestId = borrowRequestId,
                        RequesterId = studentUser.UserId,
                        RequestedQuantity = 1,
                        RequestedOn = DateTime.UtcNow,
                        ReturnDueDate = DateTime.UtcNow.AddDays(7),
                        ReturnedOn = null,
                        IsApproved = true,
                        IsReturned = false,
                        Notes = "Classroom use",
                    };

                    // adjust equipment availability safely
                    if (camera.AvailableQuantity >= borrow.RequestedQuantity)
                    {
                        camera.AvailableQuantity -= borrow.RequestedQuantity;
                    }
                    else
                    {
                        camera.AvailableQuantity = 0;
                    }

                    camera.IsAvailable = camera.AvailableQuantity > 0;

                    await context.BorrowingsAndReturns.AddAsync(borrow);
                    context.Equipments.Update(camera);
                    await context.SaveChangesAsync();
                }
                else
                {
                    // If borrow exists, optionally update status or dates (keep it minimal here)
                    existingBorrow.IsApproved = true;
                    existingBorrow.IsReturned = existingBorrow.IsReturned;
                    existingBorrow.ReturnDueDate ??= DateTime.UtcNow.AddDays(7);
                    context.BorrowingsAndReturns.Update(existingBorrow);
                    await context.SaveChangesAsync();
                }
            }

            await tx.CommitAsync();
        }

        // Simple PBKDF2 password hashing
        private static void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            passwordSalt = Convert.ToBase64String(saltBytes);

            using var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
            var hash = deriveBytes.GetBytes(32);
            passwordHash = Convert.ToBase64String(hash);
        }
    }
}