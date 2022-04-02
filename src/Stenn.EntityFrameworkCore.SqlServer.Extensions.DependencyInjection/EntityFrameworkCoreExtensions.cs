using System;
using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using CommonExtensions = Stenn.EntityFrameworkCore.Extensions.DependencyInjection.EntityFrameworkCoreExtensions;

namespace Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for register Entity Framework core static migrations
    /// </summary>
    public static class EntityFrameworkCoreExtensions
    {
        private static readonly SqlServerMigrations Configurator = new();

        /// <summary>
        ///     Use static migrations with specified db context
        /// </summary>
        /// <param name="optionsBuilder">Db contextoptions builder</param>
        /// <param name="initMigrations">Init static migrations action</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseStaticMigrationsSqlServer(this DbContextOptionsBuilder optionsBuilder,
            Action<StaticMigrationBuilder> initMigrations)
        {
            UseStaticMigrationsSqlServer(optionsBuilder, 
                options => { options.InitMigrations = initMigrations; });

            return optionsBuilder;
        }

        /// <summary>
        ///     Use static migrations with specified db context
        /// </summary>
        /// <param name="optionsBuilder">Db contextoptions builder</param>
        /// <param name="optionsInit">Static migrations options initialization</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseStaticMigrationsSqlServer(this DbContextOptionsBuilder optionsBuilder,
            Action<StaticMigrationsOptions>? optionsInit = null)
        {
            CommonExtensions.UseStaticMigrations(Configurator, optionsBuilder, optionsInit);

            return optionsBuilder;
        }
    }
}