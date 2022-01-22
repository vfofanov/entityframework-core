using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Stenn.EntityFrameworkCore
{
    public interface IStaticSqlMigration : IStaticMigration
    {
        bool FillRevertOperations(List<MigrationOperation> result);
        bool FillApplyOperations(List<MigrationOperation> result, bool isNew);
    }
}