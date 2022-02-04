using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stenn.DictionaryEntities
{
    public sealed class DictionaryEntityMigrator : IDictionaryEntityMigrator
    {
        private readonly IDictionaryEntityContext _context;
        private readonly ConcurrentDictionary<Type, DictionaryEntityInfo> _infos;

        public DictionaryEntityMigrator(IDictionaryEntityContext context)
        {
            _context = context;
            _infos = new ConcurrentDictionary<Type, DictionaryEntityInfo>();
        }

        private DictionaryEntityInfo<T> GetInfo<T>()
        {
            return (DictionaryEntityInfo<T>)_infos.GetOrAdd(typeof(T), _ => _context.GetInfo<T>());
        }

        public async Task<List<T>> AddOrUpdateAsync<T>(List<T> actualList, CancellationToken cancellationToken)
            where T : class
        {
            var info = GetInfo<T>();
            var currentList = await _context.GetCurrentAsync<T>(cancellationToken);
            foreach (var actual in actualList)
            {
                var current = currentList.FirstOrDefault(c => info.EqualsByKey(actual, c));
                if (current == null)
                {
                    await _context.AddAsync(actual, cancellationToken);
                }
                else
                {
                    currentList.Remove(current);
                    if (info.EqualsByProperties(actual, current))
                    {
                        continue;
                    }

                    info.CopyPropertiesFrom(actual, current);
                    _context.Update(current);
                }
            }
            return currentList;
        }

        /// <inheritdoc />
        public List<T> AddOrUpdate<T>(List<T> actualList) 
            where T : class
        {
            // ReSharper disable once ConstantNullCoalescingCondition
            actualList ??= new List<T>();
            
            var info = GetInfo<T>();
            var currentList = _context.GetCurrent<T>().ToList();
            foreach (var actual in actualList)
            {
                var current = currentList.FirstOrDefault(c => info.EqualsByKey(actual, c));
                if (current == null)
                {
                    _context.Add(actual);
                }
                else
                {
                    currentList.Remove(current);
                    if (info.EqualsByProperties(actual, current))
                    {
                        continue;
                    }
                    
                    info.CopyPropertiesFrom(actual, current);
                    _context.Update(current);
                }
            }
            return currentList;
        }

        public void Remove<T>(List<T> listToRemove)
            where T : class
        {
            foreach (var current in listToRemove)
            {
                _context.Remove(current);
            }
        }
    }
}