using System;
using System.Data;
using Hattem.CQRS.Sql.Tests.Framework;
using Xunit;

namespace Hattem.CQRS.Sql.Tests
{
    [Collection("Test fixture")]
    public abstract class TestsBase
    {
        private readonly TestFixture _fixture;

        protected TestsBase(TestFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        protected ISqlHattemSessionFactory CreateSessionFactory() => _fixture.CreateSessionFactory();

        protected ISqlHattemSession CreateSessionWithoutTransaction() => _fixture.CreateSessionWithoutTransaction();

        protected ISqlHattemSession CreateSession(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => _fixture.CreateSession(isolationLevel);
    }
}
