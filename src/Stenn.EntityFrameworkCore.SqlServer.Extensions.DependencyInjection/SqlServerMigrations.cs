using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SqlServer.Enums;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;
using Stenn.EntityFrameworkCore.StaticMigrations.StaticMigrations;

namespace Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection
{
    public sealed class SqlServerMigrations : RelationalDbContextOptionsConfigurator, IStaticMigrationsProviderConfigurator
    {
        /// <inheritdoc />
        public void RegisterServices(IServiceCollection services, StaticMigrationsOptions options)
        {
            services.TryAddTransient<IStaticMigrationHistoryRepository, StaticMigrationHistoryRepositorySqlServer>();

            if (options.EnableEnumTables)
            {
                services.TryAddTransient<IEnumsStaticMigrationFactory, EnumsStaticMigrationFactorySqlServer>();
            }
        }
    }
}