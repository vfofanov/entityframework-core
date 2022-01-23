using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stenn.DictionaryEntities;

namespace Stenn.EntityFrameworkCore
{
    public static class DictionaryEntityDbContextExtensions
    {
        public static IDictionaryEntityContext ToDictionaryEntityContext(this DbContext context)
        {
            return new DictionaryEntityDbContext(context);
        }

        private sealed class DictionaryEntityDbContext : IDictionaryEntityContext
        {
            private readonly DbContext _context;

            public DictionaryEntityDbContext(DbContext context)
            {
                _context = context;
            }

            /// <inheritdoc />
            public IQueryable<T> GetCurrent<T>()
                where T : class
            {
                return _context.Set<T>();
            }

            /// <inheritdoc />
            public async Task AddAsync<T>(T entity, CancellationToken cancellationToken)
                where T : class
            {
                await _context.AddAsync(entity, cancellationToken);
            }

            /// <inheritdoc />
            public void Add<T>(T entity)
                where T : class
            {
                _context.Add(entity);
            }

            /// <inheritdoc />
            public void Update<T>(T entity)
                where T : class
            {
                _context.Update(entity);
            }

            /// <inheritdoc />
            public void Remove<T>(T entity)
                where T : class
            {
                _context.Remove(entity);
            }
        }
    }
}