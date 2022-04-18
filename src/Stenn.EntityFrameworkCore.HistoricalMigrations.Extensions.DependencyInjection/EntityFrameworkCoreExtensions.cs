using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stenn.EntityFrameworkCore.HistoricalMigrations.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for register Entity Framework core static migrations
    /// </summary>
    public static class EntityFrameworkCoreExtensions
    {
        /// <summary>
        /// Add historical migrations
        /// </summary>
        /// <param name="optionsBuilder">Db context options builder</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseHistoricalMigrations(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IMigrationsAssembly, HistoricalMigrationsAssembly>();
            return optionsBuilder;
        }
    }
}