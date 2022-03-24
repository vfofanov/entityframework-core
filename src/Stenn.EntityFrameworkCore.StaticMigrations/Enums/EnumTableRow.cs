using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Enums
{
    [DebuggerDisplay("{Name}:{Value}")]
    public class EnumTableRow
    {
        public static EnumTableRow Create(Type enumType, Type valueType, MemberInfo member)
        {
            var name = member.Name;
            
            var enumValue = Enum.Parse(enumType, member.Name);
            var value = Convert.ChangeType(enumValue, valueType);
            var displayName = member.GetCustomAttribute<DisplayAttribute>() ?? 
                              new DisplayAttribute { Name = name };

            return new EnumTableRow(value, name, displayName.GetName() ?? name, displayName.GetDescription());
        }

        private EnumTableRow(object value, string name, string displayName, string? description)
        {
            Value = value;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName;
            Description = description;
        }

        public object Value { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public string? Description { get; }
    }
}