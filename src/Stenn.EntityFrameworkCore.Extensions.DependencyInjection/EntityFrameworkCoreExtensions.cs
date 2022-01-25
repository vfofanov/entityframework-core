using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for register Entity Framework core static migrations
    /// </summary>
    public static class EntityFrameworkCoreExtensions
    {
        /// <summary>
        ///     Use static migrations with specified db context
        /// </summary>
        /// <param name="optionsBuilder">Db contextoptions builder</param>
        /// <param name="initMigrations">Init static migrations action</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseStaticMigrations<TProviderRegistrator>(DbContextOptionsBuilder optionsBuilder,
            Action<StaticMigrationBuilder> initMigrations)
            where TProviderRegistrator : IDbContextOptionsConfigurator, IStaticMigrationsProviderConfigurator, new()
        {
            var configurator = new TProviderRegistrator();
            configurator.Configure(optionsBuilder);

            var migrationsBuilder = new StaticMigrationBuilder();
            initMigrations.Invoke(migrationsBuilder);

            var extension = optionsBuilder.Options.FindExtension<StaticMigrationOptionsExtension>() ??
                            new StaticMigrationOptionsExtension(configurator, migrationsBuilder);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }
    }
}