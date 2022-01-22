using System.Collections.Generic;

namespace Stenn.EntityFrameworkCore.Data.Declarations
{
    public static class RoleDeclarations
    {
        public static Role[] GetActual()
        {
            return new Role[]
            {
                new() { Id = Roles.Admin, Name = "Admin" },
                new() { Id = Roles.Customer, Name = "Customer" }
            };
        }
    }
}