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
        private IStaticMigrationsService StaticMigrationsService => _migrationsService ??= _provider.GetRequiredService<IStaticMigrationsService>();

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

        public override Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            return _databaseCreator.ExistsAsync(cancellationToken);
        }

        public override Task<bool> HasTablesAsync(CancellationToken cancellationToken = default)
        {
            return _databaseCreator.HasTablesAsync(cancellationToken);
        }

        public override Task CreateAsync(CancellationToken cancellationToken = default)
        {
            return _databaseCreator.CreateAsync(cancellationToken);
        }

        public override Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            return _databaseCreator.DeleteAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override void CreateTables()
        {
            _databaseCreator.CreateTables();
            Execute(GetMigrateOperationLists());
        }

        /// <inheritdoc />
        public override async Task CreateTablesAsync(CancellationToken cancellationToken = default)
        {
            await _databaseCreator.CreateTablesAsync(cancellationToken);
            await ExecuteAsync(GetMigrateOperationLists(), cancellationToken);
        }

        /// <inheritdoc />
        public override string GenerateCreateScript()
        {
            var sbuilder = new StringBuilder(_databaseCreator.GenerateCreateScript());
            sbuilder.AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);

            foreach (var command in GenerateCommands(GetMigrateOperationLists()))
            {
                sbuilder.AppendLine(command.CommandText);
                sbuilder.AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);
            }
            return sbuilder.ToString();
        }

        private IEnumerable<IReadOnlyList<MigrationOperation>> GetMigrateOperationLists()
        {
            var migrationDate = DateTime.UtcNow;
            
            yield return StaticMigrationsService.GetInitialOperations(migrationDate, true).ToList();
            yield return StaticMigrationsService.GetApplyOperations(migrationDate, true).ToList();
        }
        
        private IEnumerable<MigrationCommand> GenerateCommands(IEnumerable<IReadOnlyList<MigrationOperation>> operationLists)
        {
            foreach (var operationList in operationLists)
            {
                foreach (var migrationCommand in _migrationsSqlGenerator.Generate(operationList))
                {
                    yield return migrationCommand;
                }
            }
        }

        private void Execute(IEnumerable<IReadOnlyList<MigrationOperation>> operations)
        {
            var commands = GenerateCommands(operations);
            _migrationCommandExecutor.ExecuteNonQuery(commands, _connection);
        }

        private async Task ExecuteAsync(IEnumerable<IReadOnlyList<MigrationOperation>> operations, CancellationToken cancellationToken = default)
        {
            var commands = GenerateCommands(operations);
            await _migrationCommandExecutor.ExecuteNonQueryAsync(commands, _connection, cancellationToken).ConfigureAwait(false);
        }
    }
}