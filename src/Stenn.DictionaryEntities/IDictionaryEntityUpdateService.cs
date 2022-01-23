using System.Threading;
using System.Threading.Tasks;

namespace Stenn.DictionaryEntities
{
    public interface IDictionaryEntityUpdateService
    {
        string Name { get; }
        int Version { get; }
        int Order { get; }

        Task Update(IDictionaryEntityMigrator migrator, CancellationToken cancellationToken);
    }
}