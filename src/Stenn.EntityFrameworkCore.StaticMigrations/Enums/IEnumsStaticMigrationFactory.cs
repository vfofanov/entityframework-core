using Microsoft.EntityFrameworkCore;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Enums
{
    public interface IEnumsStaticMigrationFactory
    {
        IStaticSqlMigration Create(DbContext context, string schemaName = "enum");
    }
}