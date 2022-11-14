using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stenn.StaticMigrations.MigrationConditions
{
    public interface IMigrationWithTag
    {
        public string Tag { get; set; }
    }
}
