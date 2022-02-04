using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stenn.DictionaryEntities;

namespace Stenn.StaticMigrations
{
    public abstract class DictionaryEntityMigrationBase<T> : StaticMigration, IDictionaryEntityMigration
        where T : class
    {
        private DictionaryEntityInfo<T>? _info;
        public IDictionaryEntityInfoContext? InfoContext { get; set; }

        /// <inheritdoc />
        public void Update(IDictionaryEntityMigrator migrator)
        {
            migrator.Apply(GetActual());
        }

        /// <inheritdoc />
        public async Task UpdateAsync(IDictionaryEntityMigrator migrator, CancellationToken cancellationToken)
        {
            await migrator.ApplyAsync(GetActual(), cancellationToken);
        }

        protected abstract List<T> GetActual();

        protected DictionaryEntityInfo<T> GetInfo()
        {
            if (_info != null)
            {
                return _info;
            }
            if (InfoContext == null)
            {
                throw new ApplicationException("Initialize 'InfoContext' first");
            }
            return _info = InfoContext.GetInfo<T>();
        }

        protected byte[] GetHashByInfo(List<T> items)
        {
            var info = GetInfo();
            var properties = info.Keys.Concat(info.Properties).ToList();

            List<List<KeyValuePair<string, object?>>> hashItems = new(items.Count);
            foreach (var item in items)
            {
                hashItems.Add(properties.Select(property => new KeyValuePair<string, object?>(property.Name, property.GetValue(item))).ToList());
            }
            return GetHash(hashItems);
        }
    }
}