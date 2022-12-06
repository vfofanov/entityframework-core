using Stenn.EntityDefinition.Model.Definitions;

namespace Stenn.EntityDefinition.Model
{
    [DefinitionDomain(Domain.Order)]
    public class InvoiceViewExtended: InvoiceView
    {
        public string ExtraData { get; set; }

    }
}