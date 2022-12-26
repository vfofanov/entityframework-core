using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Enums
{
    [DebuggerDisplay("{TableName}, ClrType:{EnumType.Name}, ValueType: {ValueType.Name}, Rows:{Rows.Count}")]
    public class EnumTable
    {
        public static EnumTable Create<T>()
            where T : struct, Enum
        {
            var enumType = typeof(T);
            var enumTableAttribute = enumType.GetCustomAttribute<EnumTableAttribute>();
            var tableName = enumTableAttribute?.Name ?? enumType.Name;
            var valueType = enumTableAttribute?.ValueType ?? enumType.GetEnumUnderlyingType();

            var enumMembers = enumType.GetMembers(BindingFlags.Static | BindingFlags.Public);

            var rows = new List<EnumTableRow>(enumMembers.Length);
            rows.AddRange(enumMembers.Select(member => EnumTableRow.Create(enumType, valueType, member)));

            if (Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
            {
                AddValueCombinations<T>(valueType, rows);
            }

            rows.Sort((one, other) => string.Compare(one.Name, other.Name, StringComparison.Ordinal));
            return new EnumTable(tableName, enumType, valueType, rows);
        }

        public static EnumTable FromEnum(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException(nameof(enumType));
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type must be enum", nameof(enumType));
            }

            var methodInfo = typeof(EnumTable).GetMethod(nameof(Create), BindingFlags.Public | BindingFlags.Static);
            var createMethod = methodInfo!.MakeGenericMethod(enumType);

            return (EnumTable)createMethod.Invoke(null, null)!;
        }

        private static void AddValueCombinations<T>(Type valueType, List<EnumTableRow> rows) 
            where T : struct, Enum
        {
            var combos = GetEnumValueCombinations<T>();
            rows.AddRange(combos.Select(item => EnumTableRow.CreateFromCombinedEnumValue(valueType, item)));
        }
        
        private static HashSet<T> GetEnumValueCombinations<T>() 
            where T : struct, Enum
        {
            var allValues = Enum.GetValues<T>();
            HashSet<T> result = new();

            for (var i = 1; i < (1 << allValues.Length); i++)
            {
                T working = default;
                var index = 0;
                var checker = i;
                while (checker != 0)
                {
                    if ((checker & 0x01) == 0x01)
                    {
                        working = (T)Enum.ToObject(typeof(T), Convert.ToUInt64(working) | Convert.ToUInt64(allValues[index]));
                    }

                    checker >>= 1;
                    index++;
                }
                if (allValues.Contains(working))
                {
                    continue;
                }
                result.Add(working);
            }

            return result;
        }
        
        private EnumTable(string tableName, Type enumType, Type valueType, IReadOnlyList<EnumTableRow> rows)
        {
            TableName = tableName;
            EnumType = enumType;
            ValueType = valueType;
            Rows = rows;
        }

        public string TableName { get; }

        public Type ValueType { get; }

        public Type EnumType { get; }

        public IReadOnlyList<EnumTableRow> Rows { get; }
    }
}