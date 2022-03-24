using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;

namespace Stenn.EntityFrameworkCore.Tests
{
    public class EnumTableTests
    {
        [EnumTable("TestName", ValueType = typeof(short))]
        public enum EnumByte : byte
        {
            [Display(Name = "Default value")] 
            None = 0,
            [Display(Name = "First item")] 
            One = 1,
            [Display(Name = "Second value")] 
            Two = 2
        }

        [Test]
        public void ParseEnumByte()
        {
            var table = EnumTable.Create<EnumByte>();

            table.EnumType.Should().Be<EnumByte>();
            table.ValueType.Should().Be<short>();
            table.Rows.Should().HaveCount(3);

            table.Rows[0].RowShouldBe<short>(0, "None", "Default value");
            table.Rows[1].RowShouldBe<short>(1, "One", "First item");
            table.Rows[2].RowShouldBe<short>(2, "Two", "Second value");
        }
        
        public enum EnumDefault
        {
            First = 1,
            Second = 2
        }

        [Test]
        public void ParseEnumDefault()
        {
            var table = EnumTable.Create<EnumDefault>();

            table.EnumType.Should().Be<EnumDefault>();
            table.ValueType.Should().Be<int>();
            table.Rows.Should().HaveCount(2);

            table.Rows[0].RowShouldBe(1, "First", "First");
            table.Rows[1].RowShouldBe(2, "Second", "Second");
        }

        
    }
}