using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public interface IEFPropertyDefinitionInfo
    {
        DefinitionInfo Info { get; }
        object? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, IEFDefinitionExtractContext context);
    }
}