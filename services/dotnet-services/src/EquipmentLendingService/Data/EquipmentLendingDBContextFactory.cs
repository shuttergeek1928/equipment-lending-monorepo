csharp ..\EquipmentLendingService\Data\EquipmentLendingDBContextFactory.cs
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EquipmentLendingDotnetServices.Data;

public class EquipmentLendingDBContextFactory : IDesignTimeDbContextFactory<EquipmentLendingDBContext>
{
    public EquipmentLendingDBContext CreateDbContext(string[] args)
    {
        // Ensure we pick up the project's appsettings.json when PMC/ef tools run.
        var basePath = Directory.GetCurrentDirectory();
        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<EquipmentLendingDBContext>();

        var connectionString = config.GetConnectionString("DefaultConnection")
                               ?? "Host=localhost;Port=5432;Database=equipmentlendingdatabase;Username=postgres;Password=admin;";

        optionsBuilder.UseNpgsql(connectionString);

        return new EquipmentLendingDBContext(optionsBuilder.Options);
    }
}