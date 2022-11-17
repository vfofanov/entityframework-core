using Stenn.EntityDefinition.Model.Definitions;

namespace Stenn.EntityDefinition.Model
{
    [DefinitionDomain(Domain.Order)]
    public abstract class InvoiceView
    {
        public int Id { get; set; }
        public Money Fee { get; set; }

    }
}