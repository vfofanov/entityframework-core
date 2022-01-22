using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore
{
    public sealed class StaticMigrationService : IStaticMigrationService
    {
        private readonly StaticMigrationHistoryRepository _historyRepository;
        private readonly DbContext _dbContext;
        private readonly IStaticMigrationCollection<IStaticSqlMigration> _sqlMigrations;
        private readonly IStaticMigrationCollection<IDictionaryEntityMigration> _entityMigrations;

        public StaticMigrationService(StaticMigrationHistoryRepository historyRepository, DbContext dbContext, IStaticMigrationCollection<IStaticSqlMigration> sqlMigrations,
            IStaticMigrationCollection<IDictionaryEntityMigration> entityMigrations)
        {
            _historyRepository = historyRepository;
            _dbContext = dbContext;
            _sqlMigrations = sqlMigrations;
            _entityMigrations = entityMigrations;
        }
        
        /// <inheritdoc />
        public IReadOnlyList<MigrationOperation> GetDropOperationsBeforeMigrations(bool force)
        {
            if (!_historyRepository.Exists())
            {
                return ArraySegment<MigrationOperation>.Empty;
            }

            var result = new List<MigrationOperation>();

            var historyRows = _historyRepository.GetAppliedMigrations();

            var rowsForDelete = new List<StaticMigrationHistoryRow>(historyRows.Count);
            for (var i = _sqlMigrations.Count - 1; i >= 0; i--)
            {
                var sqlMigration = _sqlMigrations[i];
                var row = historyRows.FirstOrDefault(r => r.Name == sqlMigration.Name);
                if (row == null)
                {
                    continue;
                }
                if (!force && row.Hash.SequenceEqual(sqlMigration.Hash))
                {
                    continue;
                }
                if (sqlMigration.FillRevertOperations(result))
                {
                    rowsForDelete.Add(row);
                }
            }

            result.AddRange(rowsForDelete.Select(row => new SqlOperation { Sql = _historyRepository.GetDeleteScript(row) }));

            return result;
        }

        /// <inheritdoc />
        public IReadOnlyList<MigrationOperation> GetCreateOperationsAfterMigrations(bool force)
        {
            var result = new List<MigrationOperation>
            {
                new SqlOperation { Sql = _historyRepository.GetCreateIfNotExistsScript() }
            };

            var historyRows = _historyRepository.GetAppliedMigrations();

            var modifiedDate = DateTime.UtcNow;

            foreach (var sqlMigration in _sqlMigrations)
            {
                var row = historyRows.FirstOrDefault(r => r.Name == sqlMigration.Name);
                if (row != null && row.Hash.SequenceEqual(sqlMigration.Hash) && !force)
                {
                    continue;
                }
                if (row != null)
                {
                    result.Add(new SqlOperation { Sql = _historyRepository.GetDeleteScript(row) });
                }

                sqlMigration.FillApplyOperations(result, row == null);
                var historyRow = new StaticMigrationHistoryRow(sqlMigration.Name, sqlMigration.Hash, modifiedDate);
                result.Add(new SqlOperation { Sql = _historyRepository.GetInsertScript(historyRow) });
            }
            return result;
        }

        /// <inheritdoc />
        public void MigrateDictionaryEntities()
        {
            return;
            
            // var changed = await dictionaryEntityService.UpdateAsync(dbContext);
            // if (changed)
            // {
            //     await dbContext.SaveChangesAsync();
            // }
            //throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<MigrationOperation>> GetDropOperationsBeforeMigrationsAsync(bool force, CancellationToken cancellationToken)
        {
            var result = GetDropOperationsBeforeMigrations(force);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<MigrationOperation>> GetCreateOperationsAfterMigrationsAsync(bool force, CancellationToken cancellationToken)
        {
            var result = GetCreateOperationsAfterMigrations(force);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task MigrateDictionaryEntitiesAsync(CancellationToken cancellationToken)
        {
            MigrateDictionaryEntities();
            return Task.CompletedTask;
        }
    }
}