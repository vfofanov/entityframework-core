using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
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

        [Flags]
        public enum FlagsValue
        {
            One = 0x1,
            Two = 0x2,
            OneAndTwo = One | Two,
            Four = 0x4
        }

        [Test]
        public void ParseFlaggedEnum()
        {
            var table = EnumTable.Create<FlagsValue>();

            table.EnumType.Should().Be<FlagsValue>();
            table.ValueType.Should().Be<int>();

            /*
            Expected values:
            One, 
            Two,
            OneAndTwo, // Skip One | Two due existed explicit combination
            Four
            One | Four
            Two | Four
            OneAndTwo | Four // Skip One | Two | Four due existed combination with fewer values
            */
            table.Rows.Should().HaveCount(7);

            table.Rows[0].RowShouldBe(1, "One", "One");
            table.Rows[1].RowShouldBe(2, "Two", "Two");
            table.Rows[2].RowShouldBe(3, "OneAndTwo", "OneAndTwo");
            table.Rows[3].RowShouldBe(4, "Four", "Four");
            table.Rows[4].RowShouldBe(5, "One, Four", "One, Four");
            table.Rows[5].RowShouldBe(6, "Two, Four", "Two, Four");
            table.Rows[6].RowShouldBe(7, "OneAndTwo, Four", "OneAndTwo, Four");
        }
    }
}