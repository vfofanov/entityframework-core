using Microsoft.EntityFrameworkCore.Migrations;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public interface IStaticMigrationHistoryRepositoryFactory
    {
        StaticMigrationHistoryRepository Create(HistoryRepositoryDependencies dependencies);
    }
}