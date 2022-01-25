using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Stenn.StaticMigrations
{
    [DebuggerDisplay("Count={Count}")]
    public class StaticMigrationCollection<T, TContext> : IStaticMigrationCollection<T, TContext>
        where T : IStaticMigration
    {
        private readonly List<StaticMigrationItemFactory<T, TContext>> _items = new();

        public StaticMigrationItemFactory<T, TContext> this[int index] => _items[index];

        public int Count => _items.Count;

        /// <inheritdoc />
        public IEnumerator<StaticMigrationItemFactory<T, TContext>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string name, Func<TContext, T> factory)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Add(new StaticMigrationItemFactory<T,TContext>(name, factory));
        }

        public void Add(StaticMigrationItemFactory<T,TContext> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (_items.Any(itm => itm.Name == item.Name))
            {
                throw new ArgumentException($"Migration name '{item.Name}' used twice. Static migration name must be unique");
            }
            _items.Add(item);
        }
    }
}