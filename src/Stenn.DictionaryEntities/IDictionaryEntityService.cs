using System.Threading;
using System.Threading.Tasks;
using Onboarding.Domain.DictionaryEntities.Services;

namespace Onboarding.DAL.DictionaryEntities.Services
{
    public interface IDictionaryEntityService
    {
        Task<bool> UpdateAsync(IDictionaryEntityContext context, CancellationToken cancellationToken);
    }
}