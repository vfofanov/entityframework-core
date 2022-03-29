using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stenn.EntityFrameworkCore.EntityConventions;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SqlServer.Conventions;
using Stenn.EntityFrameworkCore.SqlServer.Enums;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations.Conventions;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;
using Stenn.EntityFrameworkCore.StaticMigrations.StaticMigrations;

namespace Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection
{
    public sealed class SqlServerMigrations : RelationalDbContextOptionsConfigurator, IStaticMigrationsProviderConfigurator
    {
        /// <inheritdoc />
        public void RegisterServices(IServiceCollection services)
        {
            services.TryAddTransient<IStaticMigrationHistoryRepository, StaticMigrationHistoryRepositorySqlServer>();

            services.TryAddTransient<IEnumsStaticMigrationFactory, EnumsStaticMigrationFactorySqlServer>();

            services.TryAddSingleton<IConventionsService, ConventionsServiceSqlServer>();
            services.TryAddTransient<IConventionsStaticMigrationFactory, ConventionsStaticMigrationFactorySqlServer>();
        }
    }
}