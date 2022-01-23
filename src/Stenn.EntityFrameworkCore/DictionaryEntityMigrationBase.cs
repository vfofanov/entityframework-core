using System.Threading;
using System.Threading.Tasks;
using Stenn.DictionaryEntities;

namespace Stenn.EntityFrameworkCore
{
    public abstract class DictionaryEntityMigrationBase<T> : StaticMigration, IDictionaryEntityMigration
    {
        protected DictionaryEntityMigrationBase(string name)
            : base(name)
        {
        }

        /// <inheritdoc />
        public Task Update(IDictionaryEntityMigrator migrator, CancellationToken cancellationToken)
        {
            return null;
        }

        /// <inheritdoc />
        public Task UpdateAsync(IDictionaryEntityMigrator migrator, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}