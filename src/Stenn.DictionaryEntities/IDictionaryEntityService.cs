using System.Threading;
using System.Threading.Tasks;

namespace Stenn.DictionaryEntities
{
    public interface IDictionaryEntityService
    {
        Task<bool> UpdateAsync(IDictionaryEntityContext context, CancellationToken cancellationToken);
    }
}