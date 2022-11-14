using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public interface IStaticMigrationsService
    {
        IEnumerable<MigrationOperation> GetInitialOperations(DateTime migrationDate, bool force);
        IEnumerable<MigrationOperation> GetRevertOperations(DateTime migrationDate, bool force);
        IEnumerable<MigrationOperation> GetApplyOperations(DateTime migrationDate, bool force);
        
        void CheckForSuppressTransaction(string migrationName, MigrationOperation operation);

        void FillActionTagsFrom(IReadOnlyList<Migration> efMigrations);
    }
}