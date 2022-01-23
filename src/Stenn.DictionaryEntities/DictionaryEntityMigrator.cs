using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stenn.DictionaryEntities.Contracts;

namespace Stenn.DictionaryEntities
{
    public sealed class DictionaryEntityMigrator : IDictionaryEntityMigrator
    {
        private readonly IDictionaryEntityContext _context;

        public DictionaryEntityMigrator(IDictionaryEntityContext context)
        {
            _context = context;
        }

        public async Task<List<T>> AddOrUpdateAsync<T>(List<T> actualList, CancellationToken cancellationToken)
            where T : class, IDictionaryEntity<T>
        {
            var currentList = await _context.GetCurrent<T>().ToListAsync(cancellationToken);
            foreach (var actual in actualList)
            {
                var current = currentList.FirstOrDefault(actual.EqualsByKey);
                if (current == null)
                {
                    await _context.AddAsync(actual, cancellationToken);
                }
                else
                {
                    currentList.Remove(current);
                    if (current.EqualsByProperties(actual))
                    {
                        continue;
                    }
                    
                    current.CopyPropertiesFrom(actual);
                    _context.Update(current);
                }
            }
            return currentList;
        }

        /// <inheritdoc />
        public List<T> AddOrUpdate<T>(List<T> actualList) 
            where T : class, IDictionaryEntity<T>
        {
            actualList ??= new List<T>();
            
            var currentList = _context.GetCurrent<T>().ToList();
            foreach (var actual in actualList)
            {
                var current = currentList.FirstOrDefault(actual.EqualsByKey);
                if (current == null)
                {
                    _context.Add(actual);
                }
                else
                {
                    currentList.Remove(current);
                    if (current.EqualsByProperties(actual))
                    {
                        continue;
                    }
                    
                    current.CopyPropertiesFrom(actual);
                    _context.Update(current);
                }
            }
            return currentList;
        }

        public void Remove<T>(List<T> listToRemove)
            where T : class, IDictionaryEntity<T>
        {
            foreach (var current in listToRemove)
            {
                _context.Remove(current);
            }
        }
    }
}