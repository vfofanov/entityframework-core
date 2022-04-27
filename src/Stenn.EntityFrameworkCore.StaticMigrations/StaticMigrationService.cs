using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.EntityFrameworkCore.StaticMigrations.StaticMigrations;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public class StaticMigrationsService : IStaticMigrationsService
    {
        private readonly IStaticMigrationHistoryRepository _historyRepository;
        private readonly StaticMigrationItem<IStaticSqlMigration>[] _sqlMigrations;
        private readonly StaticMigrationItem<IStaticSqlMigration>[] _initialSqlMigrations;

        public StaticMigrationsService(IStaticMigrationHistoryRepository historyRepository,
            ICurrentDbContext currentDbContext,
            IStaticMigrationCollection<IStaticSqlMigration, DbContext> sqlMigrations)
        {
            _historyRepository = historyRepository;
            var dbContext = currentDbContext.Context;


            var migrations = sqlMigrations.Select(item => new StaticMigrationItem<IStaticSqlMigration>(item.Name, item.Factory(dbContext))).ToList();
            
            _sqlMigrations = migrations.Where(m => !m.Migration.IsInitialMigration).ToArray();
            _initialSqlMigrations = migrations.Where(m => m.Migration.IsInitialMigration).ToArray();
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