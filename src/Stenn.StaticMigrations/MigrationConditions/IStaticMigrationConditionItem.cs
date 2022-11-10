using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stenn.StaticMigrations.MigrationConditions
{
    public interface IStaticMigrationConditionItem
    {
        public string Name { get; set; }
        public string Tag { get; set; }
    }
}
