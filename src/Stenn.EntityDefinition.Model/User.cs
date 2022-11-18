using System.Collections.Generic;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Model.Definitions;

namespace Stenn.EntityDefinition.Model
{
    /// <summary>
    /// User description test
    /// </summary>
    [DefinitionDomain(Domain.Security)]
    [DefinitionRemark("Users' remark")]
    public abstract class User
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// User's name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Is user active otherwise user is archived
        /// </summary>
        [DefinitionDomain(Domain.Unknown)]
        public bool IsActive { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }
    }
}