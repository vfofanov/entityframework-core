using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stenn.DictionaryEntities;
using Stenn.DictionaryEntities.Contracts;

namespace Stenn.EntityFrameworkCore
{
    public abstract class DictionaryEntityMigrationBase<T> : StaticMigration, IDictionaryEntityMigration
        where T : class, IDictionaryEntity<T>
    {
        /// <inheritdoc />
        public void Update(IDictionaryEntityMigrator migrator)
        {
            migrator.Apply(GetActual());
        }

        /// <inheritdoc />
        public async Task UpdateAsync(IDictionaryEntityMigrator migrator, CancellationToken cancellationToken)
        {
            await migrator.ApplyAsync(GetActual(), cancellationToken);
        }

        protected abstract List<T> GetActual();
    }
}