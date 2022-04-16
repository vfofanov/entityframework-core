using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stenn.EntityFrameworkCore.SplittedMigrations.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for register Entity Framework core static migrations
    /// </summary>
    public static class EntityFrameworkCoreExtensions
    {
        /// <summary>
        /// Add splitted migrations
        /// </summary>
        /// <param name="optionsBuilder">Db context options builder</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseSplittedMigrations(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IMigrationsAssembly, SplittedMigrationsAssembly>();
            return optionsBuilder;
        }
    }
}