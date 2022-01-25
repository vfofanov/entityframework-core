using System.Threading;
using System.Threading.Tasks;
using Stenn.DictionaryEntities;

namespace Stenn.StaticMigrations
{
    public interface IDictionaryEntityMigration : IStaticMigration
    {
        void Update(IDictionaryEntityMigrator migrator);
        Task UpdateAsync(IDictionaryEntityMigrator migrator, CancellationToken cancellationToken);
    }
}