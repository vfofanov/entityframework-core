using System.Collections.Generic;

namespace Stenn.EntityFrameworkCore.Data.Main.StaticMigrations.DictEntities
{
    public static class RoleDeclaration
    {
        public static List<Role> GetActual()
        {
            return new List<Role>
            {
                new() { Id = Roles.Admin, Name = "Admin" },
                new() { Id = Roles.Customer, Name = "Customer" },
                new() { Id = Roles.Support, Name = "Support" }
            };
        }
    }
}