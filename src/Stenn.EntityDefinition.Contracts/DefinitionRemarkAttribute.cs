namespace Stenn.EntityDefinition.Contracts
{
    public sealed class DefinitionRemarkAttribute : DefinitionAttribute<string>
    {
        /// <inheritdoc />
        public DefinitionRemarkAttribute(string value) 
            : base("Remark", value)
        {
        }
    }
}