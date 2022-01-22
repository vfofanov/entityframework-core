using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.DbContext.Initial
{
    public static class InitialStaticMigrations
    {
        public static void Init(StaticMigrationBuilder migrations)
        {
            migrations.AddResSql("InitDB", @"\StaticMigrations\Sql\InitDB.Apply.sql", "", suppressTransaction: true);
            migrations.AddResSql("TestViews", @"\StaticMigrations\Sql\TestViews.Apply.sql", @"StaticMigrations\Sql\TestViews.Revert.sql", suppressTransaction: true);
        }
    }
}