using Microsoft.EntityFrameworkCore;

namespace Stenn.EntityFrameworkCore.EntityConventions
{
    public interface IDbContextOptionsConfigurator
    {
        void Configure(DbContextOptionsBuilder builder);
    }
}