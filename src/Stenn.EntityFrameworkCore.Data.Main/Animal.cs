using Stenn.EntityConventions.Contacts;
using Stenn.EntityConventions.Contacts.TriggerBased;

namespace Stenn.EntityFrameworkCore.Data.Main
{
    [DiscriminatorOptions(MaxLength = 20, IsUnicode = false)]
    public abstract class Animal : IWithDiscriminatorEntityConvention<string>,
        ICreateAuditedEntityConvention,
        IUpdateAuditedEntityConvention
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