using System.Collections.Generic;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Tests.Model.Definitions;

namespace Stenn.EntityDefinition.Tests.Model
{
    [DefinitionDomain(Domain.Security)]
    [DefinitionRemark("Users' remark")]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [DefinitionDomain(Domain.Unknown)]
        public bool IsActive { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }
    }
}