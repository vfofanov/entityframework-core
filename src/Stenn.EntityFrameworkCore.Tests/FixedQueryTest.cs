using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Stenn.EntityFrameworkCore.Tests
{
    public class MigrationsTest
    {
        [Test]
        public async Task Empty()
        {
            var list = await FixedQuery<object>.Empty.ToListAsync();
            list.Should().NotBeNull();
            list.Should().BeEmpty();
        }

        [Test]
        public async Task Null()
        {
            var list = await FixedQuery<object>.Create((IEnumerable<object>)null).ToListAsync();
            list.Should().NotBeNull();
            list.Should().BeEmpty();
        }

        [Test]
        public async Task WithItems()
        {
            var list = await FixedQuery<int?>.Create(null, 1).ToListAsync();
            list.Should().NotBeNull();
            list.Should().HaveCount(2);
        }
    }
}