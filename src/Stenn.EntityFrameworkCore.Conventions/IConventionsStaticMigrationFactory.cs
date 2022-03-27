using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore.Conventions
{
    public interface IConventionsStaticMigrationFactory
    {
        IStaticSqlMigration Create(DbContext context);
    }
}