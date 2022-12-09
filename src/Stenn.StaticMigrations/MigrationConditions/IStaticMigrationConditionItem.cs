using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stenn.StaticMigrations.MigrationConditions
{
    public interface IStaticMigrationConditionItem
    {
        /// <summary>
        /// The migration was never applied to a db
        /// </summary>
        public bool IsNew { get; }
        public string Name { get; }
        public IImmutableSet<string> Tags { get; }
    }
}
