using System.Threading;
using System.Threading.Tasks;
using Onboarding.Domain.DictionaryEntities.Services;

namespace Onboarding.DAL.DictionaryEntities.Services
{
    public interface IDictionaryEntityUpdateService
    {
        string Name { get; }
        int Version { get; }
        int Order { get; }

        Task Update(IDictionaryEntityContext dbContext, CancellationToken cancellationToken);
    }
}