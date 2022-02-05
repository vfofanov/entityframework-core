namespace Stenn.EntityFrameworkCore.Data.Declarations
{
    public static class RoleDeclarations
    {
        public static Role[] GetActual()
        {
            return new Role[]
            {
                Role.Create(Roles.Admin, "Admin"),
                Role.Create(Roles.Customer, "Customer")
            };
        }
    }
}