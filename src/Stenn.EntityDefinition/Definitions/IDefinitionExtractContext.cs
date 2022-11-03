using System;
using System.Reflection;

namespace Stenn.EntityDefinition
{
    public interface IDefinitionExtractContext
    {
        T? GetAssemblyAttribute<T>(MemberInfo member)
            where T : Attribute
        {
            //TODO: Cache attributes
            return member.Module.Assembly.GetCustomAttribute<T>();
        }
    }
}