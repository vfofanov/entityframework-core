using Stenn.EntityDefinition.Tests.Model.Definitions;

namespace Stenn.EntityDefinition.Tests.Model
{
    [DefinitionDomain(Domain.Security)]
    public class UserRole
    {
        public int UserId { get; set; }
        public string RoleId { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}