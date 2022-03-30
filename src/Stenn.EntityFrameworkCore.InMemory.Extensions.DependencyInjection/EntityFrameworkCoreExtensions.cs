using System;
using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using CommonExtensions = Stenn.EntityFrameworkCore.Extensions.DependencyInjection.EntityFrameworkCoreExtensions;

namespace Stenn.EntityFrameworkCore.InMemory.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for register Entity Framework core static migrations
    /// </summary>
    public static class EntityFrameworkCoreExtensions
    {
        private static readonly InMemoryMigrations Configurator = new();

        /// <summary>
        ///     Use static migrations with specified db context
        /// </summary>
        /// <param name="optionsBuilder">Db contextoptions builder</param>
        /// <param name="initMigrations">Init static migrations action</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseStaticMigrationsInMemoryDatabase(this DbContextOptionsBuilder optionsBuilder,
            Action<StaticMigrationBuilder> initMigrations)
        {
            UseStaticMigrationsInMemoryDatabase(optionsBuilder, options =>
            {
                options.InitMigrations = initMigrations;
            });
            
            return optionsBuilder;
        }
        
        /// <summary>
        ///     Use static migrations with specified db context
        /// </summary>
        /// <param name="optionsBuilder">Db contextoptions builder</param>
        /// <param name="optionsInit">Init static migrations options</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseStaticMigrationsInMemoryDatabase(this DbContextOptionsBuilder optionsBuilder,
            Action<StaticMigrationsOptions>? optionsInit = null)
        {
            var memoryOptionsInit = (Action<StaticMigrationsOptions>)
                (options =>
                {
                    options.IncludeCommonConventions = false;
                    options.EnableEnumTables = false;

                    optionsInit?.Invoke(options);
                });

            CommonExtensions.UseStaticMigrations(Configurator, optionsBuilder, memoryOptionsInit);

            return optionsBuilder;
        }
    }
}