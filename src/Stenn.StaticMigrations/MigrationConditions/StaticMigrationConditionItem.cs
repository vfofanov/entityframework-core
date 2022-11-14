using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stenn.StaticMigrations.MigrationConditions
{
    public class StaticMigrationConditionItem : IStaticMigrationConditionItem
    {
        public string Name { get; set; } = String.Empty;
        public string Tag { get; set; } = String.Empty;
    }
}
