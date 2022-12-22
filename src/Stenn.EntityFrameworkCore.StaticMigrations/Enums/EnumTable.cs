using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Enums
{
    [DebuggerDisplay("{TableName}, ClrType:{EnumType.Name}, ValueType: {ValueType.Name}, Rows:{Rows.Count}")]
    public class EnumTable
    {
        public static EnumTable Create<T>()
            where T : Enum
        {
            return FromEnum(typeof(T));
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


            var enumTableAttribute = enumType.GetCustomAttribute<EnumTableAttribute>();
            var tableName = enumTableAttribute?.Name ?? enumType.Name;
            var valueType = enumTableAttribute?.ValueType ?? enumType.GetEnumUnderlyingType();

            var enumMembers = enumType.GetMembers(BindingFlags.Static | BindingFlags.Public);

            var rows = new List<EnumTableRow>(enumMembers.Length);
            rows.AddRange(enumMembers.Select(member => EnumTableRow.Create(enumType, valueType, member)));

            if (Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
            {
                var methodInfo = typeof(EnumTable).GetMethod("AddValueCombinations", BindingFlags.NonPublic | BindingFlags.Static);
                var addCombinationsGenericMethod = methodInfo!.MakeGenericMethod(enumType);
                addCombinationsGenericMethod.Invoke(null, new object[] { enumType, valueType, rows });
            }

            return new EnumTable(tableName, enumType, valueType, rows);
        }

        private EnumTable(string tableName, Type enumType, Type valueType, IReadOnlyList<EnumTableRow> rows)
        {
            TableName = tableName;
            EnumType = enumType;
            ValueType = valueType;
            Rows = rows;
        }

        private static void AddValueCombinations<T>(Type enumType, Type valueType, List<EnumTableRow> rows) where T : struct, Enum
        {
            var combos = GetEnumValueCombinations<T>();

            foreach (var item in combos)
            {
                if (!rows.Any(i => EqualityComparer<T>.Default.Equals((T)i.Value, item)))
                {
                    rows.Add(EnumTableRow.CreateFromCombinedEnumValue(enumType, valueType, item));
                }
            }
        }

        private static HashSet<T> GetEnumValueCombinations<T>() where T : struct, Enum
        {
            List<T> allValues = new((IEnumerable<T>)Enum.GetValues(typeof(T)));
            HashSet<T> allCombos = new();

            for (int i = 1; i < (1 << allValues.Count); i++)
            {
                T working = (T)Enum.ToObject(typeof(T), 0);
                int index = 0;
                int checker = i;
                while (checker != 0)
                {
                    if ((checker & 0x01) == 0x01)
                    {
                        working = (T)Enum.ToObject(typeof(T), Convert.ToUInt64(working) | Convert.ToUInt64(allValues[index]));
                    }

                    checker >>= 1;
                    index++;
                }
                allCombos.Add(working);
            }

            return allCombos;
        }

        public string TableName { get; }

        public Type ValueType { get; }

        public Type EnumType { get; }

        public IReadOnlyList<EnumTableRow> Rows { get; }
    }
}