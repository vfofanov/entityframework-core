using Stenn.EntityFrameworkCore.Data.Main.StaticMigrations.DictEntities;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Data.Main
{
    public static class MainStaticMigrations
    {
        public static void Init(StaticMigrationBuilder migrations)
        {
            migrations.AddDictionaryEntity(CurrencyDeclaration.GetActual);
            migrations.AddDictionaryEntity(RoleDeclaration.GetActual);
        }
    }
}