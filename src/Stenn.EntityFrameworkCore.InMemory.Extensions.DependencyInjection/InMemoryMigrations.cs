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
        public void RegisterServices(IServiceCollection services)
        {
            services.TryAddTransient<IEnumsStaticMigrationFactory, EnumsStaticMigrationFactoryInMemory>();
        }

        /// <inheritdoc />
        public void Configure(DbContextOptionsBuilder builder)
        {
        }
    }

    public class EnumsStaticMigrationFactoryInMemory : IEnumsStaticMigrationFactory
    {
        /// <inheritdoc />
        public IStaticSqlMigration Create(DbContext context, string schemaName = "enum")
        {
            return new EmptyStaticSqlMigration();
        }
    }
}