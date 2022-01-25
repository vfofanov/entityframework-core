using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.DictionaryEntities;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore
{
    public sealed class StaticMigrationsService : IStaticMigrationsService
    {
        private readonly DbContext _dbContext;
        private readonly StaticMigrationItem<IDictionaryEntityMigration>[] _entityMigrations;
        private readonly StaticMigrationHistoryRepository _historyRepository;
        private readonly IDictionaryEntityMigrator _migrator;
        private readonly StaticMigrationItem<IStaticSqlMigration>[] _sqlMigrations;

        public StaticMigrationsService(StaticMigrationHistoryRepository historyRepository,
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

        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetRevertOperations(bool force, DateTime migrationDate)
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
        public IEnumerable<MigrationOperation> GetApplyOperations(bool force, DateTime migrationDate)
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
                yield return InsertHistoryRow(migrationItem, migrationDate);
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
                result.Add(InsertHistoryRow(migrationItem, migrationDate));
            }
            if (changed)
            {
                _dbContext.SaveChanges();
            }
            return result;
        }

        /// <inheritdoc />
        public Task<IEnumerable<MigrationOperation>> GetRevertOperationsAsync(bool force, DateTime migrationDate,
            CancellationToken cancellationToken)
        {
            var result = GetRevertOperations(force, migrationDate);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<IEnumerable<MigrationOperation>> GetApplyOperationsAsync(bool force, DateTime migrationDate,
            CancellationToken cancellationToken)
        {
            var result = GetApplyOperations(force, migrationDate);
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
            return new SqlOperation { Sql = _historyRepository.GetInsertScript(new StaticMigrationHistoryRow(migration.Name, migration.Hash, migrationDate)) };
        }

        private SqlOperation DeleteHistoryRow(StaticMigrationHistoryRow row)
        {
            return new SqlOperation { Sql = _historyRepository.GetDeleteScript(row) };
        }
    }
}