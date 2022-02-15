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
    public sealed class StaticMigrationsService : IStaticMigrationsService
    {
        private readonly DbContext _dbContext;
        private readonly StaticMigrationItem<IDictionaryEntityMigration>[] _entityMigrations;
        private readonly IStaticMigrationHistoryRepository _historyRepository;
        private readonly IDictionaryEntityMigrator _migrator;
        private readonly StaticMigrationItem<IStaticSqlMigration>[] _sqlMigrations;

        public StaticMigrationsService(IStaticMigrationHistoryRepository historyRepository,
            ICurrentDbContext currentDbContext,
            IDictionaryEntityMigrator migrator,
            IStaticMigrationCollection<IStaticSqlMigration, DbContext> sqlMigrations,
            IStaticMigrationCollection<IDictionaryEntityMigration, DbContext> entityMigrations)
        {
            _historyRepository = historyRepository;
            _dbContext = currentDbContext.Context;
            _migrator = migrator;

            _sqlMigrations = sqlMigrations.Select(item => new StaticMigrationItem<IStaticSqlMigration>(item.Name, item.Factory(_dbContext))).ToArray();
            _entityMigrations = entityMigrations.Select(item => new StaticMigrationItem<IDictionaryEntityMigration>(item.Name, item.Factory(_dbContext)))
                .ToArray();
        }

        private bool GetChanges(bool force, out IReadOnlyList<StaticMigrationHistoryRow> historyRows)
        {
            historyRows = _historyRepository.GetAppliedMigrations();
            if (force)
            {
                return true;
            }
            for (var i = 0; i < _sqlMigrations.Length; i++)
            {
                var migrationItem = _sqlMigrations[i];
                var row = historyRows.FirstOrDefault(r => r.Name == migrationItem.Name);
                if (row == null || !row.Hash.SequenceEqual(migrationItem.GetHash()))
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetRevertOperations(DateTime migrationDate, bool force)
        {
            if (!_historyRepository.Exists())
            {
                for (var i = _sqlMigrations.Length - 1; i >= 0; i--)
                {
                    var migrationItem = _sqlMigrations[i];
                    foreach (var operation in migrationItem.Migration.GetRevertOperations())
                    {
                        yield return operation;
                    }
                }
                yield break;
            }
            
            var hasChanges = GetChanges(force, out var historyRows);
            if (!hasChanges)
            {
                yield break;
            }
            for (var i = _sqlMigrations.Length - 1; i >= 0; i--)
            {
                var (name, migration) = _sqlMigrations[i];
                var row = historyRows.FirstOrDefault(r => r.Name == name);
                if (row == null)
                {
                    continue;
                }
                var deleteRow = false;
                foreach (var operation in migration.GetRevertOperations())
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
        public IEnumerable<MigrationOperation> GetApplyOperations(DateTime migrationDate, bool force)
        {
            yield return CreateIfNotExistsHistoryTable();
            var hasChanges = GetChanges(force, out var historyRows);
            if (!hasChanges)
            {
                yield break;
            }

            for (var i = 0; i < _sqlMigrations.Length; i++)
            {
                var migration = _sqlMigrations[i];
                var row = historyRows.FirstOrDefault(r => r.Name == migration.Name);
                if (row != null)
                {
                    yield return DeleteHistoryRow(row);
                }
                foreach (var operation in migration.Migration.GetApplyOperations(row == null))
                {
                    yield return operation;
                }
                yield return InsertHistoryRow(migration, migrationDate);
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