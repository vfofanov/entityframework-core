#nullable enable
using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Stenn.EntityDefinition.InMemory.Tests
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [TestCase(typeof(int), "Int32")]
        [TestCase(typeof(int?), "Int32?")]
        [TestCase(typeof(List<int?>), "List<Int32?>")]
        [TestCase(typeof(List<object>), "List<Object>")]
        [TestCase(typeof(List<object?>), "List<Object>")]
        public void HumanizeNameTest(Type type, string expected)
        {
            var actual = type.HumanizeName();
            actual.Should().Be(expected);
        }
    }
}