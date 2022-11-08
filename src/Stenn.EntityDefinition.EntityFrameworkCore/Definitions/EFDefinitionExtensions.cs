using System.Runtime.InteropServices.ComTypes;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public static class EFDefinitionExtensions
    {
        public static EFEntityDefinition<T> ToEntity<T>(this MemberInfoDefinition<T> definition)
        {
            return new EFEntityDefinition<T>(definition.Info, (type,parentValue, context) => definition.Extract(type.ClrType, parentValue, context));
        }

        public static EFPropertyDefinition<T> ToProperty<T>(this MemberInfoDefinition<T> info)
        {
            return new EFPropertyDefinition<T>(info.Info, (_, propertyInfo, _, parentValue, context) => info.Extract(propertyInfo, parentValue, context));
        }
    }
}