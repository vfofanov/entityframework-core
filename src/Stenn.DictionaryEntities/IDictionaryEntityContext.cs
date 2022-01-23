using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stenn.DictionaryEntities
{
    public interface IDictionaryEntityContext
    {
        IQueryable<T> GetCurrent<T>()
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