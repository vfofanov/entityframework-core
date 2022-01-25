using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore
{
    public interface IStaticSqlMigration : IStaticMigration
    {
        IEnumerable<MigrationOperation> GetRevertOperations();
        IEnumerable<MigrationOperation> GetApplyOperations(bool isNew);
    }
}