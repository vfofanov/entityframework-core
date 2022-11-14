using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stenn.StaticMigrations.MigrationConditions
{
    public class StaticMigrationConditionOptions
    {
        public StaticMigrationConditionOptions(List<IStaticMigrationConditionItem> changedMigrations, List<string> migrationTags)
        {
            ChangedMigrations = changedMigrations;
            MigrationTags = migrationTags;
        }

        public List<string> MigrationTags { get; } = new();

        public List<IStaticMigrationConditionItem> ChangedMigrations { get; } = new();
    }
}
