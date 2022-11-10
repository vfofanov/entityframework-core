using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stenn.StaticMigrations.MigrationConditions
{
    public class StaticMigrationConditionOptions
    {
        public StaticMigrationConditionOptions(List<IStaticMigrationConditionItem> changedMigrations)
        {
            ChangedMigrations = changedMigrations;
        }

        public List<string> ChangedMigrationNames { get; } = new();

        public List<IStaticMigrationConditionItem> ChangedMigrations { get; } = new();
    }
}
