using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public interface IStaticMigrationsService
    {
        IEnumerable<MigrationOperation> GetRevertOperations(DateTime migrationDate, bool force);
        IEnumerable<MigrationOperation> GetApplyOperations(DateTime migrationDate, bool force);
        IReadOnlyList<MigrationOperation> MigrateDictionaryEntities(DateTime migrationDate, bool force = false);
        
        Task<IEnumerable<MigrationOperation>> GetRevertOperationsAsync(DateTime migrationDate, bool force, CancellationToken cancellationToken);
        Task<IEnumerable<MigrationOperation>> GetApplyOperationsAsync(DateTime migrationDate, bool force, CancellationToken cancellationToken);
        Task<IReadOnlyList<MigrationOperation>> MigrateDictionaryEntitiesAsync(DateTime migrationDate, CancellationToken cancellationToken, bool force = false);
    }
}