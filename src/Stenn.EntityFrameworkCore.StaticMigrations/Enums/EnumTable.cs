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
            return new EnumTable(tableName, enumType, valueType, rows);
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