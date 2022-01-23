using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Stenn.EntityFrameworkCore
{
    public interface IStaticMigrationService
    {
        IReadOnlyList<MigrationOperation> GetDropOperationsBeforeMigrations(bool force);
        IReadOnlyList<MigrationOperation> GetCreateOperationsAfterMigrations(bool force);
        IReadOnlyList<MigrationOperation> MigrateDictionaryEntities(bool force = false);
        
        Task<IReadOnlyList<MigrationOperation>> GetDropOperationsBeforeMigrationsAsync(bool force, CancellationToken cancellationToken);
        Task<IReadOnlyList<MigrationOperation>> GetCreateOperationsAfterMigrationsAsync(bool force, CancellationToken cancellationToken);
        Task MigrateDictionaryEntitiesAsync(CancellationToken cancellationToken, bool force = false);
    }
}