using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Stenn.EntityFrameworkCore
{
    public interface IStaticMigrationService
    {
        IEnumerable<MigrationOperation> GetRevertOperations(bool force);
        IEnumerable<MigrationOperation> GetApplyOperations(bool force);
        IReadOnlyList<MigrationOperation> MigrateDictionaryEntities(bool force = false);
        
        Task<IEnumerable<MigrationOperation>> GetRevertOperationsAsync(bool force, CancellationToken cancellationToken);
        Task<IEnumerable<MigrationOperation>> GetApplyOperationsAsync(bool force, CancellationToken cancellationToken);
        Task<IReadOnlyList<MigrationOperation>> MigrateDictionaryEntitiesAsync(CancellationToken cancellationToken, bool force = false);
    }
}