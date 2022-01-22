using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stenn.EntityFrameworkCore
{
    public interface IStaticMigrationServiceFactory
    {
        IStaticMigrationService Create(DbContext dbContext, HistoryRepositoryDependencies dependencies);
    }
}