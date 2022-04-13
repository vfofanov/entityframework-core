using Stenn.EntityFrameworkCore.Data.Main.StaticMigrations.DictEntities;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Data.Main.StaticMigrations
{
    public static class MainStaticMigrations
    {
        public static void Init(StaticMigrationBuilder migrations)
        {
            migrations.AddInitResSql("InitDB", @"\StaticMigrations\Sql\InitDB.Apply.sql", suppressTransaction: true);
            
            migrations.AddResSql("TestViews", @"\StaticMigrations\Sql\TestViews.Apply.sql", @"StaticMigrations\Sql\TestViews.Revert.sql");

            InitDictEntities(migrations);
        }

        private static void InitDictEntities(StaticMigrationBuilder migrations)
        {
            migrations.AddDictionaryEntity(CurrencyDeclaration.GetActual);
            migrations.AddDictionaryEntity(RoleDeclaration.GetActual);
        }
    }
}