using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Stenn.EntityFrameworkCore
{
    public interface IStaticSqlMigration : IStaticMigration
    {
        bool FillRevertOperations(List<MigrationOperation> operations);
        void FillApplyOperations(List<MigrationOperation> operations, bool isNew);
    }
}