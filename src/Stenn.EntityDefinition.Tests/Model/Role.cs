using System.Collections.Generic;
using Stenn.EntityDefinition.Tests.Model.Definitions;

namespace Stenn.EntityDefinition.Tests.Model
{
    [DefinitionDomain(Domain.Security)]
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<UserRole> Users { get; set; }
    }
}