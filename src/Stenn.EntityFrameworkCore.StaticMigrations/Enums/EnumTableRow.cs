using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
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
            var display = member.GetCustomAttribute<DisplayAttribute>();
            var displayName = member.GetCustomAttribute<DisplayNameAttribute>();
            var description = member.GetCustomAttribute<DescriptionAttribute>();

            return new EnumTableRow(value, enumValue, name,
                display?.GetName() ?? displayName?.DisplayName ?? name,
                display?.GetDescription() ?? description?.Description ?? string.Empty);
        }

        public static EnumTableRow CreateFromCombinedEnumValue(Type enumType, Type valueType, object enumItem)
        {
            string name = SortCombinedEnumName(enumItem.ToString() ?? string.Empty);
            var value = Convert.ChangeType(enumItem, valueType);
            string rawValue = value.ToString() ?? string.Empty;

            return new EnumTableRow(value, rawValue, name, name, string.Empty, true);
        }

        private static string SortCombinedEnumName(string name)
        {
            var parts = name.Split(',').Select(p => p.Trim()).ToList();
            parts.Sort();
            return String.Join(", ", parts);
        }

        private EnumTableRow(object value, object rawValue, string name, string displayName, string description, bool isCombined = false)
        {
            Value = value;
            RawValue = rawValue;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName;
            Description = description;
            IsCombined = isCombined;
        }

        public object Value { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public object RawValue { get; }
        public bool IsCombined { get; }
    }
}