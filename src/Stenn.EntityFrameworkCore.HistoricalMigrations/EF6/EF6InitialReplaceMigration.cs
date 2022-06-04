using System;
using System.Collections.Generic;
using System.Linq;
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
            //TODO: Make extendable by Service
            switch (builder.ActiveProvider)
            {
                case "Microsoft.EntityFrameworkCore.SqlServer":
                {
                    var migrationIdValues = string.Join(", ", RemoveMigrationRowIds.Select(r => $"('{r}')"));
                    var sql = $@"
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'[dbo].[__MigrationHistory]') AND type IN (N'U'))
    BEGIN
        THROW 51000, 'Could not find EF6 history table [dbo].[__MigrationHistory]. ', 1
    END
GO
DROP TABLE IF EXISTS #EF6AppliedMigrations
CREATE TABLE #EF6AppliedMigrations
(
    MigrationId nvarchar(150) NOT NULL PRIMARY KEY
)
GO
DROP TABLE IF EXISTS __MigrationHistoryEF6ToEFCoreDelta
CREATE TABLE __MigrationHistoryEF6ToEFCoreDelta
(
    MigrationId nvarchar(150) NOT NULL PRIMARY KEY,
    IsMissed    bit           NOT NULL DEFAULT (0),
    IsExtra     bit           NOT NULL DEFAULT (0)
)
GO
INSERT INTO #EF6AppliedMigrations
VALUES
{migrationIdValues}
GO

INSERT INTO __MigrationHistoryEF6ToEFCoreDelta (MigrationId, IsExtra)
SELECT  h.MigrationId, 1
FROM dbo.__MigrationHistory     h
LEFT JOIN #EF6AppliedMigrations m ON h.MigrationId = m.MigrationId
WHERE m.MigrationId IS NULL

INSERT INTO __MigrationHistoryEF6ToEFCoreDelta (MigrationId, IsMissed)
SELECT  m.MigrationId, 1
FROM #EF6AppliedMigrations       m
LEFT JOIN dbo.__MigrationHistory h ON m.MigrationId = h.MigrationId
WHERE h.MigrationId IS NULL
GO
IF (EXISTS(SELECT * FROM __MigrationHistoryEF6ToEFCoreDelta WHERE IsExtra = 1))
    BEGIN
        THROW 64400, 'EXTRA: Database has applied migrations missed from EF Core initial migration. See details in __MigrationHistoryEF6ToEFCoreDelta table', 1;
    END

IF (EXISTS(SELECT * FROM __MigrationHistoryEF6ToEFCoreDelta WHERE IsMissed = 1))
    BEGIN
        THROW 64400, 'MISSED. Database does not have applied migrations existed in EF Core initial migration. See details in __MigrationHistoryEF6ToEFCoreDelta table', 1;
    END

DROP TABLE IF EXISTS __MigrationHistoryEF6ToEFCoreDelta
DROP TABLE IF EXISTS dbo.__MigrationHistory
GO
DROP TABLE IF EXISTS #EF6AppliedMigrations
";
                    builder.Sql(sql);
                    break;
                }
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    throw new NotImplementedException("Migrate from EF6 to EF Core doesn't implemented for PostgreSQL");
                default:
                    throw new EF6MigrateException($"Unexpected provider for Migrate from EF6 to EF Core. Provider:{builder.ActiveProvider}");

            }
        }
    }
}