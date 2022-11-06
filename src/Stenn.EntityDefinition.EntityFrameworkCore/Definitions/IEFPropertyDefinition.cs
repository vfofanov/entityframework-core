using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public interface IEFPropertyDefinition: IDefinition
    {
        object? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, object? parentValue, DefinitionContext context);
    }
}