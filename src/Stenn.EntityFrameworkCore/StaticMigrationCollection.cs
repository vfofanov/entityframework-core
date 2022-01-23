using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Stenn.EntityFrameworkCore
{
    [DebuggerDisplay("Count={Count}")]
    public class StaticMigrationCollection<T> : IStaticMigrationCollection<T>
        where T : IStaticMigration
    {
        private readonly List<StaticMigrationItemFactory<T>> _items = new();

        public StaticMigrationItemFactory<T> this[int index] => _items[index];

        public int Count => _items.Count;

        /// <inheritdoc />
        public IEnumerator<StaticMigrationItemFactory<T>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string name, Func<DbContext, T> factory)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Add(new StaticMigrationItemFactory<T>(name, factory));
        }

        public void Add(StaticMigrationItemFactory<T> item)
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


    public interface IStaticMigrationCollection<T> : IEnumerable<StaticMigrationItemFactory<T>>
    {
        StaticMigrationItemFactory<T> this[int index] { get; }
        int Count { get; }
    }
}