using Microsoft.EntityFrameworkCore;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Conventions
{
    public interface IConventionsStaticMigrationFactory
    {
        IStaticSqlMigration Create(DbContext context);
    }
}