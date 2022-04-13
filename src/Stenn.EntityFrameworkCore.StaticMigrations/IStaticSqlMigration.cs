using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public interface IStaticSqlMigration : IStaticMigration
    {
        /// <summary>
        /// Is migration initial. It will run only once at the start of migration and can be run with supress transaction
        /// </summary>
        bool IsInitialMigration => false;
        IEnumerable<MigrationOperation> GetRevertOperations();
        IEnumerable<MigrationOperation> GetApplyOperations();
    }
}