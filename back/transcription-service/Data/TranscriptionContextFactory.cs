using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TranscriptionService.Data;

public sealed class TranscriptionContextFactory : IDesignTimeDbContextFactory<TranscriptionContext>
{
    public TranscriptionContext CreateDbContext(string[] args)
    {

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                  ?? "Development";
        
        var cfg = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        var cs = cfg.GetConnectionString("Postgres")
                 ?? throw new InvalidOperationException(
                     "ConnectionStrings:Postgres not found.");
        
        var opts = new DbContextOptionsBuilder<TranscriptionContext>()
            .UseNpgsql(cs, o => o.MigrationsAssembly(typeof(TranscriptionContext).Assembly.FullName))
            .EnableSensitiveDataLogging()
            .Options;

        return new TranscriptionContext(opts);
    }
}