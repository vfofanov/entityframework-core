using Microsoft.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.EntityConventions
{
    public interface IEntityConventionsProviderConfigurator
    {
        void RegisterServices(IServiceCollection services, EntityConventionsOptions options);
    }
}