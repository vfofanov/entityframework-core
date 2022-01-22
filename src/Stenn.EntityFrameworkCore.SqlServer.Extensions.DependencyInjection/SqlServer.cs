using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SqlServer.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection
{
    public sealed class SqlServerMigrations : IProviderRegistrator
    {
        /// <inheritdoc />
        public void RegisterServices(IServiceCollection services)
        {
            services.TryAddTransient<IDbContextOptionsConfigurator, RelationalDbContextOptionsConfigurator>();
            services.TryAddTransient<IStaticMigrationHistoryRepositoryFactory, StaticMigrationHistoryRepositoryFactorySqlServer>();
        }
    }
}