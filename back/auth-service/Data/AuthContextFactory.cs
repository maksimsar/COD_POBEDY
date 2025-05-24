using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthService.Data
{
    /// <summary>
    /// Инструментальная фабрика для EF Core миграций (design-time).
    /// </summary>
    public class AuthContextFactory
        : IDesignTimeDbContextFactory<AuthContext>
    {
        public AuthContext CreateDbContext(string[] args)
        {
            // 1. Читаем конфиг из appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())   // auth-service/
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                .Build();

            // 2. Берём connection string (ключ "Postgres")
            var conn = configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException(
                           "ConnectionStrings:Postgres не найден");

            // 3. Настраиваем опции DbContext
            var options = new DbContextOptionsBuilder<AuthContext>()
                .UseNpgsql(conn, npgsql =>
                {
                    npgsql.MigrationsHistoryTable("__EFMigrationsHistory");
                })
                .Options;

            // 4. Возвращаем новый экземпляр
            return new AuthContext(options);
        }
    }
}