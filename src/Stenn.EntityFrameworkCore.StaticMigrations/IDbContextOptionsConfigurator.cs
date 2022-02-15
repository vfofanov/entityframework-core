using Microsoft.EntityFrameworkCore;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public interface IDbContextOptionsConfigurator
    {
        void Configure(DbContextOptionsBuilder builder);
    }
}