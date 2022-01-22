using Microsoft.EntityFrameworkCore;

namespace Stenn.EntityFrameworkCore
{
    public interface IDbContextOptionsConfigurator
    {
        void Configure(DbContextOptionsBuilder builder);
    }
}