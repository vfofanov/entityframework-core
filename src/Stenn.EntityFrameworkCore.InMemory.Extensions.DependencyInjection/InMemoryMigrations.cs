using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore.InMemory.Extensions.DependencyInjection
{
    public sealed class InMemoryMigrations :IDbContextOptionsConfigurator, IStaticMigrationsProviderConfigurator
    {
        public void RegisterServices(IServiceCollection services)
        {
        }

        /// <inheritdoc />
        public void Configure(DbContextOptionsBuilder builder)
        {
        }
    }
}