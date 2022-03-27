using Stenn.EntityFrameworkCore.Data.Main.StaticMigrations.DictEntities;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Data.Main.StaticMigrations
{
    public static class MainStaticMigrations
    {
        public static void Init(StaticMigrationBuilder migrations)
        {
            migrations.InitConventions();
            
            migrations.AddEnumTables();
            
            migrations.AddResSql("TestViews", @"\StaticMigrations\Sql\TestViews.Apply.sql", @"StaticMigrations\Sql\TestViews.Revert.sql",
                suppressTransaction: true);

            InitDictEntities(migrations);
        }

        private static void InitDictEntities(StaticMigrationBuilder migrations)
        {
            migrations.AddDictionaryEntity(CurrencyDeclaration.GetActual);
            migrations.AddDictionaryEntity(RoleDeclaration.GetActual);
        }
    }
}