using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Stenn.DictionaryEntities;

namespace Stenn.EntityFrameworkCore
{
    public sealed class DbContextDictionaryEntityMigrator : IDictionaryEntityMigrator
    {
        private readonly DictionaryEntityMigrator _migrator;

        public DbContextDictionaryEntityMigrator(ICurrentDbContext currentDbContext)
        {
            _migrator = new DictionaryEntityMigrator(currentDbContext.Context.ToDictionaryEntityContext());
        }

        /// <inheritdoc />
        public Task<List<T>> AddOrUpdateAsync<T>(List<T> actualList, CancellationToken cancellationToken) where T : class, IDictionaryEntity<T>
        {
            return _migrator.AddOrUpdateAsync(actualList, cancellationToken);
        }

        /// <inheritdoc />
        public List<T> AddOrUpdate<T>(List<T> actualList) where T : class, IDictionaryEntity<T>
        {
            return _migrator.AddOrUpdate(actualList);
        }

        /// <inheritdoc />
        public void Remove<T>(List<T> listToRemove) where T : class, IDictionaryEntity<T>
        {
            _migrator.Remove(listToRemove);
        }
    }
}