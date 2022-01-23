using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.DictionaryEntities;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore
{
    public sealed class StaticMigrationService : IStaticMigrationService
    {
        private readonly StaticMigrationHistoryRepository _historyRepository;
        private readonly DbContext _dbContext;
        private readonly IDictionaryEntityMigrator _migrator;
        private readonly StaticMigrationItem<IStaticSqlMigration>[] _sqlMigrations;
        private readonly StaticMigrationItem<IDictionaryEntityMigration>[] _entityMigrations;
        private DateTime? _modifiedDate;

        public StaticMigrationService(StaticMigrationHistoryRepository historyRepository, 
            DbContext dbContext,
            IDictionaryEntityMigrator migrator,
            StaticMigrationItem<IStaticSqlMigration>[] sqlMigrations,
            StaticMigrationItem<IDictionaryEntityMigration>[] entityMigrations)
        {
            _historyRepository = historyRepository;
            _dbContext = dbContext;
            _migrator = migrator;
            _sqlMigrations = sqlMigrations;
            _entityMigrations = entityMigrations;
        }
        
        private DateTime Modified => _modifiedDate ??= DateTime.UtcNow;
        
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
            for (var i = _sqlMigrations.Length - 1; i >= 0; i--)
            {
                var migrationItem = _sqlMigrations[i];
                var row = historyRows.FirstOrDefault(r => r.Name == migrationItem.Name);
                if (row == null)
                {
                    continue;
                }
                if (!force && row.Hash.SequenceEqual(migrationItem.Hash))
                {
                    continue;
                }
                if (migrationItem.Migration.FillRevertOperations(result))
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
            var result = CreateMigrationOperations();
            var historyRows = _historyRepository.GetAppliedMigrations();
            
            foreach (var migrationItem in _sqlMigrations)
            {
                var row = historyRows.FirstOrDefault(r => r.Name == migrationItem.Name);
                if (row != null && row.Hash.SequenceEqual(migrationItem.Hash) && !force)
                {
                    continue;
                }
                if (row != null)
                {
                    result.Add(DeleteHistoryRow(row));
                }
                migrationItem.Migration.FillApplyOperations(result, row == null);
                result.Add(InsertHistoryRow(migrationItem));
            }
            return result;
        }

        private List<MigrationOperation> CreateMigrationOperations()
        {
            var result = new List<MigrationOperation>
            {
                new SqlOperation { Sql = _historyRepository.GetCreateIfNotExistsScript() }
            };
            return result;
        }
        private SqlOperation InsertHistoryRow<T>(StaticMigrationItem<T> migration) 
            where T : IStaticMigration
        {
            return new SqlOperation { Sql = _historyRepository.GetInsertScript(new StaticMigrationHistoryRow(migration.Name, migration.Hash, Modified)) };
        }
        private SqlOperation DeleteHistoryRow(StaticMigrationHistoryRow row)
        {
            return new SqlOperation { Sql = _historyRepository.GetDeleteScript(row) };
        }

        /// <param name="force"></param>
        /// <inheritdoc />
        public IReadOnlyList<MigrationOperation> MigrateDictionaryEntities(bool force = false)
        {
            var result = CreateMigrationOperations();
            var historyRows = _historyRepository.GetAppliedMigrations();
            var changed = false;
                
            foreach (var migrationItem in _entityMigrations)
            {
                var row = historyRows.FirstOrDefault(r => r.Name == migrationItem.Name);
                if (row != null && row.Hash.SequenceEqual(migrationItem.Hash) && !force)
                {
                    continue;
                }
                if (row != null)
                {
                    result.Add(DeleteHistoryRow(row));
                }
                changed = true;
                migrationItem.Migration.Update(_migrator);
                result.Add(InsertHistoryRow(migrationItem));
            }
            if (changed)
            {
                _dbContext.SaveChanges();
            }
            return result;
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
        public Task MigrateDictionaryEntitiesAsync(CancellationToken cancellationToken, bool force = false)
        {
            MigrateDictionaryEntities();
            return Task.CompletedTask;
        }
    }
}