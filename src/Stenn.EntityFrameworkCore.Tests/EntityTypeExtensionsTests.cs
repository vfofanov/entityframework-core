using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.Data.Initial;
using Stenn.EntityFrameworkCore.Relational;

namespace Stenn.EntityFrameworkCore.Tests
{
    public class EntityTypeExtensionsTests
    {
        [Test]
        public void IsView()
        {
            var context = new InitialDbContextFactory().CreateDbContext(Array.Empty<string>());

            var currency = context.Model.GetEntityTypes().First(t => t.ClrType == typeof(CurrencyV1));
            currency.IsView().Should().BeFalse();
            
            var vcurrency = context.Model.GetEntityTypes().First(t => t.ClrType == typeof(VCurrency));
            vcurrency.IsView().Should().BeTrue();
        }

        [Test]
        public void IsTable()
        {
            var context = new InitialDbContextFactory().CreateDbContext(Array.Empty<string>());

            var currency = context.Model.GetEntityTypes().First(t => t.ClrType == typeof(CurrencyV1));
            currency.IsTable().Should().BeTrue();
            
            var vcurrency = context.Model.GetEntityTypes().First(t => t.ClrType == typeof(VCurrency));
            vcurrency.IsTable().Should().BeFalse();
        }
    }
}