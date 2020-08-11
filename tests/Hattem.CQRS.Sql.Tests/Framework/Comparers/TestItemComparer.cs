using System;
using System.Collections.Generic;
using Hattem.CQRS.Sql.Tests.Framework.Models;

namespace Hattem.CQRS.Sql.Tests.Framework.Comparers
{
    public sealed class TestItemComparer : IEqualityComparer<TestItem>
    {
        public static TestItemComparer Default { get; } = new TestItemComparer();

        public bool Equals(TestItem x, TestItem y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Id == y.Id
                && x.Value == y.Value;
        }

        public int GetHashCode(TestItem obj)
        {
            throw new NotImplementedException();
        }
    }
}
