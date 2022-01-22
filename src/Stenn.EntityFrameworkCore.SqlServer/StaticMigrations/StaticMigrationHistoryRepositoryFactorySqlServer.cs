using Microsoft.EntityFrameworkCore.Migrations;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore.SqlServer.StaticMigrations
{
    public sealed class StaticMigrationHistoryRepositoryFactorySqlServer : IStaticMigrationHistoryRepositoryFactory
    {
        /// <inheritdoc />
        public StaticMigrationHistoryRepository Create(HistoryRepositoryDependencies dependencies)
        {
            return new SqlServerStaticMigrationHistoryRepository(dependencies);
        }
    }
}