using Microsoft.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    public interface IProviderRegistrator
    {
        void RegisterServices(IServiceCollection services);
    }
}