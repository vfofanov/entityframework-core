using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Data.Main.HistoricalWithoutAttribute.Migrations.Static
{
    public static class MainStaticMigrations
    {
        public static void Init(StaticMigrationBuilder migrations)
        {
            migrations.AddInitialSqlResFile("InitDB", suppressTransaction: true);

            migrations.AddSqlResFile("TestViews");
            migrations.AddSqlResFile("vCurrency");
        }
    }
}