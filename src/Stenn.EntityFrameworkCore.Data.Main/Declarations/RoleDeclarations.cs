namespace Stenn.EntityFrameworkCore.Data.Main.Declarations
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