using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stenn.DictionaryEntities
{
    public interface IDictionaryEntityContext : IDictionaryEntityInfoContext
    {
        List<T> GetCurrent<T>()
            where T : class;

        Task<List<T>> GetCurrentAsync<T>(CancellationToken cancellationToken)
            where T : class;

        Task AddAsync<T>(T entity, CancellationToken cancellationToken)
            where T : class;

        void Add<T>(T entity)
            where T : class;

        void Remove<T>(T entity)
            where T : class;

        void Update<T>(T entity)
            where T : class;
    }
}