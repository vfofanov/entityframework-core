using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Stenn.EntityFrameworkCore
{
    #pragma warning disable EF1001
    public class MigratorWithStaticMigrations : Migrator
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IMigrationsSqlGenerator _migrationsSqlGenerator;
        private readonly IMigrationCommandExecutor _migrationCommandExecutor;
        private readonly IRelationalConnection _connection;
        private readonly ICurrentDbContext _currentContext;
        private readonly IRelationalDatabaseCreator _databaseCreator;
        private readonly HistoryRepositoryDependencies _dependencies;

        /// <inheritdoc />
        public MigratorWithStaticMigrations(IMigrationsAssembly migrationsAssembly, IHistoryRepository historyRepository, IDatabaseCreator databaseCreator,
            IMigrationsSqlGenerator migrationsSqlGenerator, IRawSqlCommandBuilder rawSqlCommandBuilder, IMigrationCommandExecutor migrationCommandExecutor,
            IRelationalConnection connection, ISqlGenerationHelper sqlGenerationHelper, ICurrentDbContext currentContext,
            IConventionSetBuilder conventionSetBuilder, IDiagnosticsLogger<DbLoggerCategory.Migrations> logger,
            IDiagnosticsLogger<DbLoggerCategory.Database.Command> commandLogger, IDatabaseProvider databaseProvider,
            IRelationalDatabaseCreator databaseCreator2, HistoryRepositoryDependencies dependencies)
            : base(migrationsAssembly, historyRepository, databaseCreator, migrationsSqlGenerator,
                rawSqlCommandBuilder, migrationCommandExecutor, connection, sqlGenerationHelper, currentContext, conventionSetBuilder, logger, commandLogger,
                databaseProvider)
        {
            _historyRepository = historyRepository;
            _migrationsSqlGenerator = migrationsSqlGenerator;
            _migrationCommandExecutor = migrationCommandExecutor;
            _connection = connection;
            _currentContext = currentContext;
            _databaseCreator = databaseCreator2;
            _dependencies = dependencies;
        }

        /// <inheritdoc />
        public override void Migrate(string? targetMigration = null)
        {
            var service = GetStaticMigrationsService();
            if (service == null)
            {
                base.Migrate(targetMigration);
                return;
            }
            
            if (targetMigration != null)
            {
                throw new ArgumentException("Migrate to targetMigration not supported", nameof(targetMigration));
            }
            
            if (HasMigrationsToApply(_historyRepository.GetAppliedMigrations()))
            {
                if (_historyRepository.Exists() && _databaseCreator.Exists())
                {
                    Execute(service.GetDropOperationsBeforeMigrations(true));
                }
                base.Migrate(targetMigration);
                Execute(service.GetCreateOperationsAfterMigrations(true));
            }
            else
            {
                Execute(service.GetDropOperationsBeforeMigrations(false));
                Execute(service.GetCreateOperationsAfterMigrations(false));
            }
            service.MigrateDictionaryEntities();
        }

        /// <inheritdoc />
        public override async Task MigrateAsync(string? targetMigration = null, CancellationToken cancellationToken = default)
        {
            var service = GetStaticMigrationsService();
            if (service == null)
            {
                await base.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);
                return;
            }

            if (targetMigration != null)
            {
                throw new ArgumentException("Migrate to targetMigration not supported", nameof(targetMigration));
            }
            
            if (HasMigrationsToApply(await _historyRepository.GetAppliedMigrationsAsync(cancellationToken).ConfigureAwait(false)))
            {
                if (await _historyRepository.ExistsAsync(cancellationToken) &&
                    await _databaseCreator.ExistsAsync(cancellationToken))
                {
                    await ExecuteAsync(await service.GetDropOperationsBeforeMigrationsAsync(true, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
                }

                await base.MigrateAsync(string.Empty, cancellationToken).ConfigureAwait(false);

                await ExecuteAsync(await service.GetCreateOperationsAfterMigrationsAsync(true, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await ExecuteAsync(await service.GetDropOperationsBeforeMigrationsAsync(false, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
                await ExecuteAsync(await service.GetCreateOperationsAfterMigrationsAsync(false, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
            }
            await service.MigrateDictionaryEntitiesAsync(cancellationToken).ConfigureAwait(false);
        }

        private IStaticMigrationService? GetStaticMigrationsService()
        {
            return _currentContext.Context.GetService<IStaticMigrationServiceFactory>()?.Create(_currentContext.Context, _dependencies);
        }
        
        private bool HasMigrationsToApply(IEnumerable<HistoryRow> appliedMigrationEntries)
        {
            PopulateMigrations(appliedMigrationEntries.Select(t => t.MigrationId),
                string.Empty,
                out var migrationsToApply,
                out _,
                out _);

            return migrationsToApply.Count != 0;
        }

        private void Execute(IReadOnlyList<MigrationOperation> operations)
        {
            var commands = _migrationsSqlGenerator.Generate(operations);
            _migrationCommandExecutor.ExecuteNonQuery(commands, _connection);
        }

        private async Task ExecuteAsync(IReadOnlyList<MigrationOperation> operations, CancellationToken cancellationToken = default)
        {
            var commands = _migrationsSqlGenerator.Generate(operations);
            await _migrationCommandExecutor.ExecuteNonQueryAsync(commands, _connection, cancellationToken).ConfigureAwait(false);
        }
    }
}