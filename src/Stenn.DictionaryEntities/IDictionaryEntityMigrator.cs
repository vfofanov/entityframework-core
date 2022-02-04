using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stenn.DictionaryEntities
{
    public interface IDictionaryEntityMigrator
    {
        /// <summary>
        /// Add or update entities
        /// </summary>
        /// <param name="actualList"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>List entities to remove</returns>
        Task<List<T>> AddOrUpdateAsync<T>(List<T> actualList, CancellationToken cancellationToken)
            where T : class;
        
        /// <summary>
        /// Add or update entities
        /// </summary>
        /// <param name="actualList"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>List entities to remove</returns>
        List<T> AddOrUpdate<T>(List<T> actualList)
            where T : class;
        
        void Remove<T>(List<T> listToRemove)
            where T : class;
        
        public void Apply<T>(List<T> actualList)
            where T : class
        {
            var toRemoveList = AddOrUpdate(actualList);
            Remove(toRemoveList);
        }
        
        public async Task ApplyAsync<T>(List<T> actualList, CancellationToken cancellationToken)
            where T : class
        {
            var toRemoveList = await AddOrUpdateAsync(actualList, cancellationToken);
            Remove(toRemoveList);
        }
    }
}