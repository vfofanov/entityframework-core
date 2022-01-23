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
        public IEnumerable<MigrationOperation> GetRevertOperations(bool force)
        {
            if (!_historyRepository.Exists())
            {
                yield break;
            }
            var historyRows = _historyRepository.GetAppliedMigrations();
            for (var i = _sqlMigrations.Length - 1; i >= 0; i--)
            {
                var migrationItem = _sqlMigrations[i];
                var row = historyRows.FirstOrDefault(r => r.Name == migrationItem.Name);
                if (row == null || !force && row.Hash.SequenceEqual(migrationItem.Hash))
                {
                    continue;
                }
                var deleteRow = false;
                foreach (var operation in migrationItem.Migration.GetRevertOperations())
                {
                    deleteRow = true;
                    yield return operation;

                }
                if (deleteRow)
                {
                    yield return new SqlOperation { Sql = _historyRepository.GetDeleteScript(row) };
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetApplyOperations(bool force)
        {
            yield return CreateIfNotExistsHistoryTable();
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
                    yield return DeleteHistoryRow(row);
                }
                foreach (var operation in migrationItem.Migration.GetApplyOperations(row == null))
                {
                    yield return operation;
                }
                yield return InsertHistoryRow(migrationItem);
            }
        }

        private SqlOperation CreateIfNotExistsHistoryTable()
        {
            return new SqlOperation { Sql = _historyRepository.GetCreateIfNotExistsScript() };
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
            var result = new List<MigrationOperation> { CreateIfNotExistsHistoryTable() };

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
        public Task<IEnumerable<MigrationOperation>> GetRevertOperationsAsync(bool force, CancellationToken cancellationToken)
        {
            var result = GetRevertOperations(force);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<IEnumerable<MigrationOperation>> GetApplyOperationsAsync(bool force, CancellationToken cancellationToken)
        {
            var result = GetApplyOperations(force);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<MigrationOperation>> MigrateDictionaryEntitiesAsync(CancellationToken cancellationToken, bool force = false)
        {
            var result = MigrateDictionaryEntities();
            return Task.FromResult(result);
        }
    }
}