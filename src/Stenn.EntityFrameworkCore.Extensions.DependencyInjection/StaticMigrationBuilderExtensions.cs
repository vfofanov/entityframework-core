using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    public static class StaticMigrationBuilderExtensions
    {
        public static void AddEnumTables(this StaticMigrationBuilder builder, string name = "#Enums", string enumSchemaName = "enum")
        {
            builder.AddStaticSqlFactory(name,
                context =>
                {
                    var factory = context.GetInfrastructure().GetService<IEnumsStaticMigrationFactory>();
                    if (factory == null)
                    {
                        throw new EnumStaticMigrationException("Db provider doesn't support enum tables");
                    }
                    return factory.Create(context, enumSchemaName);
                });
        }
    }
}