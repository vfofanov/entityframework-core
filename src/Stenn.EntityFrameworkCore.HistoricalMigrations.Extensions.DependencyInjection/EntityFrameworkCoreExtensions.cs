using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        /// <param name="options">Historical migrations' options</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseHistoricalMigrations(this DbContextOptionsBuilder optionsBuilder, HistoricalMigrationsOptions? options = null)
        {
            var extension = optionsBuilder.Options.FindExtension<HistoricalMigrationsOptionsExtension>();
            if (extension != null)
            {
                throw new InvalidOperationException("Historical migrations are already registered");
            }

            options ??= new HistoricalMigrationsOptions();
            extension = new HistoricalMigrationsOptionsExtension(options);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            optionsBuilder.ReplaceService<IMigrationsAssembly, HistoricalMigrationsAssembly>();
            return optionsBuilder;
        }

        /// <summary>
        /// Add historical migrations and specify DbContext type
        /// </summary>
        /// <param name="optionsBuilder">Db context options builder</param>
        /// <param name="options">Historical migrations' options</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseHistoricalMigrations<TDbContext>(this DbContextOptionsBuilder optionsBuilder, HistoricalMigrationsOptions? options = null)
            where TDbContext : DbContext
        {
            var extension = optionsBuilder.Options.FindExtension<HistoricalMigrationsOptionsExtension>();
            if (extension != null)
            {
                throw new InvalidOperationException("Historical migrations are already registered");
            }

            options ??= new HistoricalMigrationsOptions();
            options.DbContextType = typeof(TDbContext);
            extension = new HistoricalMigrationsOptionsExtension(options);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            optionsBuilder.ReplaceService<IMigrationsAssembly, HistoricalMigrationsAssembly>();
            return optionsBuilder;
        }
    }
}