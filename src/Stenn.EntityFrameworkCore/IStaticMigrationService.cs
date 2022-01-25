using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Stenn.EntityFrameworkCore
{
    public interface IStaticMigrationsService
    {
        IEnumerable<MigrationOperation> GetRevertOperations(bool force, DateTime migrationDate);
        IEnumerable<MigrationOperation> GetApplyOperations(bool force, DateTime migrationDate);
        IReadOnlyList<MigrationOperation> MigrateDictionaryEntities(DateTime migrationDate, bool force = false);
        
        Task<IEnumerable<MigrationOperation>> GetRevertOperationsAsync(bool force, DateTime migrationDate, CancellationToken cancellationToken);
        Task<IEnumerable<MigrationOperation>> GetApplyOperationsAsync(bool force, DateTime migrationDate, CancellationToken cancellationToken);
        Task<IReadOnlyList<MigrationOperation>> MigrateDictionaryEntitiesAsync(DateTime migrationDate, CancellationToken cancellationToken, bool force = false);
    }
}