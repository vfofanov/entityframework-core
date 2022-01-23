using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stenn.DictionaryEntities;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for register Entity Framework core static migrations
    /// </summary>
    public static class EntityFrameworkCoreExtensions
    {
        /// <summary>
        ///     Adds services for static megrations
        /// </summary>
        /// <param name="services">Services' collection</param>
        /// <param name="initMigrations">Init static migrations action</param>
        public static IServiceCollection AddStaticMigrations<TProviderRegistrator>(this IServiceCollection services,
            Action<StaticMigrationBuilder> initMigrations) where TProviderRegistrator : IProviderRegistrator, new()
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (initMigrations == null)
            {
                throw new ArgumentNullException(nameof(initMigrations));
            }
            var providerRegistrator = new TProviderRegistrator();
            providerRegistrator.RegisterServices(services);

            services.TryAddTransient<IStaticMigrationServiceFactory, StaticMigrationServiceFactory>();
            services.TryAddTransient<IDictionaryEntityMigratorFactory, DictionaryEntityMigratorFactory>();
            
            var builder = new StaticMigrationBuilder();
            initMigrations.Invoke(builder);
            builder.Build(services);

            return services;
        }

        /// <summary>
        /// Use static migrations with specified db context
        /// </summary>
        /// <param name="builder">Db contextoptions builder</param>
        /// <param name="provider">Application service provider</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseStaticMigrations(this DbContextOptionsBuilder builder, IServiceProvider provider)
        {
            var configurator = provider.GetRequiredService<IDbContextOptionsConfigurator>();
            configurator.Configure(builder);
            return builder;
        }
    }
}