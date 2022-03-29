using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.EntityConventions;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations.Conventions;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    public static class StaticMigrationBuilderExtensions
    {
        /// <summary>
        /// Add support for enum tables. All enums used in enity model will be extracted as a tables to database
        /// and referenced to depend columns via foreign keys
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="enumSchemaName"></param>
        /// <exception cref="EnumStaticMigrationException"></exception>
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

        /// <summary>
        /// Initialize support for conventions on static migrations side
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <exception cref="StaticMigrationException"></exception>
        public static void InitConventions(this StaticMigrationBuilder builder, string name = "#Conventions")
        {
            builder.AddStaticSqlFactory(name,
                context =>
                {
                    var factory = context.GetInfrastructure().GetService<IConventionsStaticMigrationFactory>();
                    if (factory == null)
                    {
                        throw new StaticMigrationException("Db provider doesn't support conventions");
                    }
                    return factory.Create(context);
                });
        }
    }
}