using Xunit;

namespace Hattem.CQRS.Sql.Tests.Framework
{
    [CollectionDefinition("Test fixture")]
    public sealed class TestCollection : ICollectionFixture<TestFixture>
    {

    }
}
