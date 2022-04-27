using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;

namespace Stenn.EntityFrameworkCore.InMemory.Extensions.DependencyInjection
{
    public class EnumsStaticMigrationFactoryInMemory : IEnumsStaticMigrationFactory
    {
        /// <inheritdoc />
        public IStaticSqlMigration Create(DbContext context, string schemaName = "enum")
        {
            return new EmptyStaticSqlMigration();
        }
    }
}