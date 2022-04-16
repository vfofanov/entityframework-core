using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stenn.EntityFrameworkCore.SplittedMigrations
{
    public abstract class InitialMigrationReplaceBase : Migration
    {
        private readonly IHistoryRepository _historyRepository;

        /// <inheritdoc />
        protected InitialMigrationReplaceBase(IHistoryRepository historyRepository, List<string> removeMigrationRowIds)
        {
            _historyRepository = historyRepository;
            RemoveMigrationRowIds = removeMigrationRowIds;
        }

        public List<string> RemoveMigrationRowIds { get; }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var migrationId in RemoveMigrationRowIds)
            {
                var sql = _historyRepository.GetDeleteScript(migrationId);
                migrationBuilder.Sql(sql);
            }

        }
    }
}