using System;
using Hattem.CQRS.Sql.Tests.Framework.Models;

namespace Hattem.CQRS.Sql.Tests.Framework.Generators
{
    public sealed class TestItemGenerator
    {
        public static TestItem Generate(int id = 0)
        {
            return new TestItem
            {
                Id = id,
                Value = Guid.NewGuid().ToString()
            };
        }
    }
}
