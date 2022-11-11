using Stenn.EntityDefinition.EntityFrameworkCore;

namespace Stenn.EntityDefinition.AspNetCore
{
    public interface ICsvEntityFrameworkCoreDefinitionOptions : IEntityFrameworkCoreDefinitionOptions
    {
        char Delimiter { get; set; }
    }

    public sealed class CsvEntityFrameworkCoreDefinitionOptions : EntityFrameworkCoreDefinitionOptions, ICsvEntityFrameworkCoreDefinitionOptions
    {
        public char Delimiter { get; set; } = ',';
    }
}