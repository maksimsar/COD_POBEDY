using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MetadataService.Data;

public class MetadataContextFactory
    : IDesignTimeDbContextFactory<MetadataContext>
{
    public MetadataContext CreateDbContext(string[] args)
    {
        // Убедитесь, что файл настроек называется appsettings.Development.json
        var basePath = Directory.GetCurrentDirectory();
        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        // Имя строки подключения должно совпадать с тем, что в вашем appsettings
        var connectionString = config.GetConnectionString("DefaultConnection") 
                               ?? config.GetConnectionString("Postgres");

        var builder = new DbContextOptionsBuilder<MetadataContext>();
        builder.UseNpgsql(connectionString, o => o.EnableRetryOnFailure());

        return new MetadataContext(builder.Options);
    }
}