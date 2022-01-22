using Microsoft.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    /// <summary>
    /// Builder for configure static migrations
    /// </summary>
    public sealed class StaticMigrationBuilder
    {
        private readonly IServiceCollection _services;

        public StaticMigrationBuilder(IServiceCollection services)
        {
            _services = services;
        }
        
        
    }
}