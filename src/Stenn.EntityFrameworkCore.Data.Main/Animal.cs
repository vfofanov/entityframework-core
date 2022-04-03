using Stenn.EntityConventions.Contacts;

namespace Stenn.EntityFrameworkCore.Data.Main
{
    [DiscriminatorOptions(MaxLength = 20, IsUnicode = false)]
    public abstract class Animal : IEntityWithDiscriminator<string>,
        ICreateAuditedEntity,
        IUpdateAuditedEntity
    {
        public int Id { get; set; }
    }

    public class Elefant : Animal
    {
    }

    public class Cat : Animal
    {
    }
}