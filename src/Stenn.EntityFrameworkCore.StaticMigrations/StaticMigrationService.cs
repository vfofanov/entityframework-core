using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.DictionaryEntities;
using Stenn.EntityFrameworkCore.StaticMigrations.StaticMigrations;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public class StaticMigrationsService : IStaticMigrationsService
    {
        private readonly DbContext _dbContext;
        private readonly StaticMigrationItem<IDictionaryEntityMigration>[] _entityMigrations;
        private readonly IStaticMigrationHistoryRepository _historyRepository;
        private readonly IDictionaryEntityMigrator _migrator;
        private readonly StaticMigrationItem<IStaticSqlMigration>[] _sqlMigrations;
        private readonly StaticMigrationItem<IStaticSqlMigration>[] _initialSqlMigrations;

        public StaticMigrationsService(IStaticMigrationHistoryRepository historyRepository,
            ICurrentDbContext currentDbContext,
            IDictionaryEntityMigrator migrator,
            IStaticMigrationCollection<IStaticSqlMigration, DbContext> sqlMigrations,
            IStaticMigrationCollection<IDictionaryEntityMigration, DbContext> entityMigrations)
        {
            _historyRepository = historyRepository;
            _dbContext = currentDbContext.Context;
            _migrator = migrator;


            var migrations = sqlMigrations.Select(item => new StaticMigrationItem<IStaticSqlMigration>(item.Name, item.Factory(_dbContext))).ToList();
            
            _sqlMigrations = migrations.Where(m => !m.Migration.IsInitialMigration).ToArray();
            _initialSqlMigrations = migrations.Where(m => m.Migration.IsInitialMigration).ToArray();

            _entityMigrations = entityMigrations.Select(item => new StaticMigrationItem<IDictionaryEntityMigration>(item.Name, item.Factory(_dbContext)))
                .ToArray();
        }

        private bool GetChanges(StaticMigrationItem<IStaticSqlMigration>[] items, bool force, out IReadOnlyList<StaticMigrationHistoryRow> historyRows)
        {
            historyRows = _historyRepository.GetAppliedMigrations();
            if (force)
            {
                return true;
            }
            for (var i = 0; i < items.Length; i++)
            {
                var migrationItem = items[i];
                var row = historyRows.FirstOrDefault(r => r.Name == migrationItem.Name);
                if (row == null || !row.Hash.SequenceEqual(migrationItem.GetHash()))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void CheckForSuppressTransaction(string migrationName, MigrationOperation operation)
        {
            if (operation is SqlOperation { SuppressTransaction: true })
            {
                throw new StaticMigrationException(
                    $"Migration: {migrationName}. SqlOperation.SuppressTransaction == true only allowed in Initial static migrations. Rewrite your migration to use SuppressTransaction in initial static migrations only.");
            }
        }
        
        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetInitialOperations(DateTime migrationDate, bool force)
        {
            var migrationItems = _initialSqlMigrations;
            
            if (!_historyRepository.Exists())
            {
                yield return CreateIfNotExistsHistoryTable();
                
                for (var i = 0; i < migrationItems.Length; i++)
                {
                    var item = migrationItems[i];
                    foreach (var operation in item.Migration.GetApplyOperations())
                    {
                        yield return operation;
                    }
                    yield return InsertHistoryRow(item, migrationDate);
                }
                yield break;
            }

            var hasChanges = GetChanges(migrationItems, force, out var historyRows);
            if (!hasChanges)
            {
                yield break;
            }

            for (var i = 0; i < migrationItems.Length; i++)
            {
                var item = migrationItems[i];
                
                var row = historyRows.FirstOrDefault(r => r.Name == item.Name);
                if (row != null)
                {
                    yield return DeleteHistoryRow(row);
                }
                foreach (var operation in item.Migration.GetApplyOperations())
                {
                    yield return operation;
                }
                yield return InsertHistoryRow(item, migrationDate);
            }
        }

        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetRevertOperations(DateTime migrationDate, bool force)
        {
            var migrationItems = _sqlMigrations;
            
            if (!_historyRepository.Exists())
            {
                for (var i = migrationItems.Length - 1; i >= 0; i--)
                {
                    var (name, migration) = migrationItems[i];
                    foreach (var operation in migration.GetRevertOperations())
                    {
                        CheckForSuppressTransaction($"{name}(Static Revert)", operation);
                        yield return operation;
                    }
                }
                yield break;
            }
            
            var hasChanges = GetChanges(migrationItems, force, out var historyRows) ||
                             GetChanges(_initialSqlMigrations, force, out _);
            if (!hasChanges)
            {
                yield break;
            }
            for (var i = migrationItems.Length - 1; i >= 0; i--)
            {
                var (name, migration) = migrationItems[i];
                var row = historyRows.FirstOrDefault(r => r.Name == name);

                if (row != null)
                {
                    yield return DeleteHistoryRow(row);
                }
                foreach (var operation in migration.GetRevertOperations())
                {
                    CheckForSuppressTransaction($"{name}(Static Revert)", operation);
                    yield return operation;
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetApplyOperations(DateTime migrationDate, bool force)
        {
            var migrationItems = _sqlMigrations;
            
            yield return CreateIfNotExistsHistoryTable();
            var hasChanges = GetChanges(migrationItems, force, out var historyRows) ||
                             GetChanges(_initialSqlMigrations, force, out _);
            if (!hasChanges)
            {
                yield break;
            }

            for (var i = 0; i < migrationItems.Length; i++)
            {
                var item = migrationItems[i];
                var row = historyRows.FirstOrDefault(r => r.Name == item.Name);
                if (row != null)
                {
                    yield return DeleteHistoryRow(row);
                }
                foreach (var operation in item.Migration.GetApplyOperations())
                {
                    CheckForSuppressTransaction($"{item.Name}(Static Apply)", operation);
                    yield return operation;
                }
                yield return InsertHistoryRow(item, migrationDate);
            }
        }

        /// <param name="migrationDate"></param>
        /// <param name="force"></param>
        /// <inheritdoc />
        public IReadOnlyList<MigrationOperation> MigrateDictionaryEntities(DateTime migrationDate, bool force = false)
        {
            var result = new List<MigrationOperation> { CreateIfNotExistsHistoryTable() };

            var historyRows = _historyRepository.GetAppliedMigrations();
            var changed = false;

            foreach (var migrationItem in _entityMigrations)
            {
                var row = historyRows.FirstOrDefault(r => r.Name == migrationItem.Name);
                if (row != null && row.Hash.SequenceEqual(migrationItem.GetHash()) && !force)
                {
                    continue;
                }
                if (row != null)
                {
                    result.Add(DeleteHistoryRow(row));
                }
                changed = true;
                migrationItem.Migration.Update(_migrator);
                result.Add(InsertHistoryRow(migrationItem, migrationDate));
            }
            if (changed)
            {
                _dbContext.SaveChanges();
            }
            return result;
        }

        /// <inheritdoc />
        public Task<IEnumerable<MigrationOperation>> GetInitialOperationsAsync(DateTime migrationDate, bool force, CancellationToken cancellationToken)
        {
            var result = GetInitialOperations(migrationDate, force);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<IEnumerable<MigrationOperation>> GetRevertOperationsAsync(DateTime migrationDate,
            bool force,
            CancellationToken cancellationToken)
        {
            var result = GetRevertOperations(migrationDate, force);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<IEnumerable<MigrationOperation>> GetApplyOperationsAsync(DateTime migrationDate,
            bool force,
            CancellationToken cancellationToken)
        {
            var result = GetApplyOperations(migrationDate, force);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<MigrationOperation>> MigrateDictionaryEntitiesAsync(DateTime migrationDate, CancellationToken cancellationToken,
            bool force = false)
        {
            var result = MigrateDictionaryEntities(migrationDate);
            return Task.FromResult(result);
        }

        private SqlOperation CreateIfNotExistsHistoryTable()
        {
            return new SqlOperation { Sql = _historyRepository.GetCreateIfNotExistsScript() };
        }

        private SqlOperation InsertHistoryRow<T>(StaticMigrationItem<T> migration, DateTime migrationDate)
            where T : IStaticMigration
        {
            return new SqlOperation { Sql = _historyRepository.GetInsertScript(new StaticMigrationHistoryRow(migration.Name, migration.GetHash(), migrationDate)) };
        }

        private SqlOperation DeleteHistoryRow(StaticMigrationHistoryRow row)
        {
            return new SqlOperation { Sql = _historyRepository.GetDeleteScript(row) };
        }
    }
}