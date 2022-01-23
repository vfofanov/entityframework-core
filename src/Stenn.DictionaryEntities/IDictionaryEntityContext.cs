using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Onboarding.Domain.DictionaryEntities.Services
{
    public interface IDictionaryEntityContext
    {
        IQueryable<T> GetCurrent<T>()
            where T : class;
        Task AddAsync<T>(T entity, CancellationToken cancellationToken)
            where T : class;
        
        void Remove<T>(T entity)
            where T : class;

        void Update<T>(T entity) 
            where T : class;
    }
}