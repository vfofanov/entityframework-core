using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stenn.EntityFrameworkCore.HistoricalMigrations.EF6
{
    public abstract class EF6InitialReplaceMigration : ReplaceMigrationBase
    {
        /// <inheritdoc />
        protected EF6InitialReplaceMigration(
            IHistoryRepository historyRepository,
            IReadOnlyCollection<string> removeMigrationRowIds)
            : base(historyRepository, removeMigrationRowIds)
        {
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder builder)
        {
            builder.DropTable("__MigrationHistory");
        }
    }
}