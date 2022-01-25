using Microsoft.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    public interface IStaticMigrationsProviderConfigurator
    {
        void RegisterServices(IServiceCollection services);
    }
}