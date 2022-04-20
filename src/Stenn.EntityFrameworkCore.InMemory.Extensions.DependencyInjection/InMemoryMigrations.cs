using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;

namespace Stenn.EntityFrameworkCore.InMemory.Extensions.DependencyInjection
{
    public sealed class InMemoryMigrations :IDbContextOptionsConfigurator, IStaticMigrationsProviderConfigurator
    {
        public void RegisterServices(IServiceCollection services, StaticMigrationsOptions options)
        {
            if (options.EnableEnumTables)
            {
                services.TryAddTransient<IEnumsStaticMigrationFactory, EnumsStaticMigrationFactoryInMemory>();
            }
        }

        /// <inheritdoc />
        public void Configure(DbContextOptionsBuilder builder)
        {
        }
    }
}