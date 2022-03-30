using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Stenn.EntityFrameworkCore.StaticMigrations;

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
        /// <param name="configurator">Specific provider registration</param>
        /// <param name="optionsBuilder">Db contextoptions builder</param>
        /// <param name="optionsInit">Static migrations options initialization</param>
        /// <returns></returns>
        public static void UseStaticMigrations<TProviderRegistrator>(TProviderRegistrator configurator, DbContextOptionsBuilder optionsBuilder,
            Action<StaticMigrationsOptions>? optionsInit)
            where TProviderRegistrator : IDbContextOptionsConfigurator, IStaticMigrationsProviderConfigurator, new()
        {
            var extension = optionsBuilder.Options.FindExtension<StaticMigrationOptionsExtension>();
            if (extension != null)
            {
                return;
            }
            configurator.Configure(optionsBuilder);

            
            var options = new StaticMigrationsOptions();
            optionsInit?.Invoke(options);
            
            extension = new StaticMigrationOptionsExtension(configurator, options);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
        }
    }
}