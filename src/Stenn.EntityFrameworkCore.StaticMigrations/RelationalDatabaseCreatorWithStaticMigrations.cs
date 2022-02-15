using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public class RelationalDatabaseCreatorWithStaticMigrations : RelationalDatabaseCreator
    {
        private readonly IRelationalConnection _connection;
        private readonly IRelationalDatabaseCreator _databaseCreator;
        private readonly IMigrationCommandExecutor _migrationCommandExecutor;
        private readonly IMigrationsSqlGenerator _migrationsSqlGenerator;
        private readonly IServiceProvider _provider;
        
        private IStaticMigrationsService? _migrationsService;
        private IStaticMigrationsService MigrationsService => _migrationsService ??= _provider.GetRequiredService<IStaticMigrationsService>();

        /// <inheritdoc />
        public RelationalDatabaseCreatorWithStaticMigrations(IRelationalDatabaseCreator databaseCreator,
            IServiceProvider provider,
            RelationalDatabaseCreatorDependencies dependencies,
            IRelationalConnection connection,
            IMigrationCommandExecutor migrationCommandExecutor,
            IMigrationsSqlGenerator migrationsSqlGenerator)
            : base(dependencies)
        {
            _connection = connection;
            _databaseCreator = databaseCreator;
            _migrationCommandExecutor = migrationCommandExecutor;
            _migrationsSqlGenerator = migrationsSqlGenerator;
            _provider = provider;
        }

        /// <inheritdoc />
        public override bool Exists()
        {
            return _databaseCreator.Exists();
        }

        /// <inheritdoc />
        public override bool HasTables()
        {
            return _databaseCreator.HasTables();
        }

        /// <inheritdoc />
        public override void Create()
        {
            _databaseCreator.Create();
        }

        /// <inheritdoc />
        public override void Delete()
        {
            _databaseCreator.Delete();
        }

        public override Task<bool> ExistsAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _databaseCreator.ExistsAsync(cancellationToken);
        }

        public override Task<bool> HasTablesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _databaseCreator.HasTablesAsync(cancellationToken);
        }

        public override Task CreateAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _databaseCreator.CreateAsync(cancellationToken);
        }

        public override Task DeleteAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _databaseCreator.DeleteAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override void CreateTables()
        {
            _databaseCreator.CreateTables();
            
            var migrationDate = DateTime.UtcNow;
            Execute(MigrationsService.GetApplyOperations(migrationDate, true).ToList());
            Execute(MigrationsService.MigrateDictionaryEntities(migrationDate, true));
        }

        /// <inheritdoc />
        public override async Task CreateTablesAsync(CancellationToken cancellationToken = default)
        {
            await _databaseCreator.CreateTablesAsync(cancellationToken);

            var migrationDate = DateTime.UtcNow;
            await ExecuteAsync((await MigrationsService.GetApplyOperationsAsync(migrationDate, true, cancellationToken)).ToList(), cancellationToken);
            await ExecuteAsync(await MigrationsService.MigrateDictionaryEntitiesAsync(migrationDate, cancellationToken, true), cancellationToken);
        }

        /// <inheritdoc />
        public override string GenerateCreateScript()
        {
            var sbuilder = new StringBuilder(_databaseCreator.GenerateCreateScript());
            sbuilder.AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);

            var migrationDate = DateTime.UtcNow;
            foreach (var command in GenerateCommands(MigrationsService.GetApplyOperations(migrationDate, true).ToList()))
            {
                sbuilder.AppendLine(command.CommandText);
                sbuilder.AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);
            }
            return sbuilder.ToString();
        }

        private IEnumerable<MigrationCommand> GenerateCommands(IReadOnlyList<MigrationOperation> operations)
        {
            return _migrationsSqlGenerator.Generate(operations);
        }

        private void Execute(IReadOnlyList<MigrationOperation> operations)
        {
            var commands = GenerateCommands(operations);
            _migrationCommandExecutor.ExecuteNonQuery(commands, _connection);
        }

        private async Task ExecuteAsync(IReadOnlyList<MigrationOperation> operations, CancellationToken cancellationToken = default)
        {
            if (operations.Count == 0)
            {
                return;
            }
            var commands = GenerateCommands(operations);
            await _migrationCommandExecutor.ExecuteNonQueryAsync(commands, _connection, cancellationToken).ConfigureAwait(false);
        }
    }
}