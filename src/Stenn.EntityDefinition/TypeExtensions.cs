using System;
using System.Linq;
using System.Reflection;

namespace Stenn.EntityDefinition
{
    internal static class TypeExtensions
    {
        public static string HumanizeName(this TypeInfo type)
        {
            var name = type.Name;
            if (!type.IsGenericType)
            {
                return name;
            }

            if (type.IsValueType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0].HumanizeName() + "?";
            }
            return $"{name[..name.IndexOf('`')]}<{string.Join(", ", type.GetGenericArguments().Select(HumanizeName))}>";
        }

        public static string HumanizeName(this Type type)
        {
            return type.GetTypeInfo().HumanizeName();
        }
    }
}