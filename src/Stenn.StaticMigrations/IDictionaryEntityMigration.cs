using System.Threading;
using System.Threading.Tasks;
using Stenn.DictionaryEntities;

namespace Stenn.StaticMigrations
{
    public interface IDictionaryEntityMigration : IStaticMigration
    {
        IDictionaryEntityInfoContext? InfoContext { get; set; }
        
        void Update(IDictionaryEntityMigrator migrator);
        Task UpdateAsync(IDictionaryEntityMigrator migrator, CancellationToken cancellationToken);
    }
}