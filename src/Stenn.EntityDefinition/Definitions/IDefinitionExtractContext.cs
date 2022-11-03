using System;
using System.Reflection;

namespace Stenn.EntityDefinition.Definitions
{
    public interface IDefinitionExtractContext
    {
        T? GetParentAttribute<T>(MemberInfo member)
            where T : Attribute
        {
            if (member.MemberType == MemberTypes.TypeInfo)
            {
                //TODO: Cache attributes

                return member.Module.Assembly.GetCustomAttribute<T>();
            }
            return null;
        }
    }
}