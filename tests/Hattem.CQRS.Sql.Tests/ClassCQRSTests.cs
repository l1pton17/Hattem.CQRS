using System;
using System.Threading.Tasks;
using Hattem.CQRS.Sql.Extensions;
using Hattem.CQRS.Sql.Tests.Framework;
using Hattem.CQRS.Sql.Tests.Framework.Commands.TestItems;
using Hattem.CQRS.Sql.Tests.Framework.Comparers;
using Hattem.CQRS.Sql.Tests.Framework.Extensions;
using Hattem.CQRS.Sql.Tests.Framework.Generators;
using Hattem.CQRS.Sql.Tests.Framework.Notifications.Handlers.TestItems;
using Hattem.CQRS.Sql.Tests.Framework.Queries.TestItems;
using Xunit;

namespace Hattem.CQRS.Sql.Tests
{
    [CategoryTrait("CQRS class")]
    public sealed class ClassCQRSTests : TestsBase
    {
        public ClassCQRSTests(TestFixture fixture)
            : base(fixture)
        {
        }

        [Fact(DisplayName = "Should execute command with return")]
        public async Task ExecuteCommandAndReturn()
        {
            await using var session = CreateSession();

            var expected = await session
                .ExecuteCommandAndReturn(new AddTestItemCommand(TestItemGenerator.Generate()))
                .AssertAndGet();

            var actual = await session
                .ProcessQuery(new GetTestItemQuery(expected.Id))
                .AssertAndGet();

            Assert.Equal(expected, actual, TestItemComparer.Default);

            await session.CommitAsync();
        }

        [Fact(DisplayName = "Should execute query")]
        public async Task Query()
        {
            var expected = await CreateSessionFactory()
                .Execute(session => session.ExecuteCommandAndReturn(new AddTestItemCommand(TestItemGenerator.Generate())))
                .AssertAndGet();

            var actual = await CreateSessionFactory()
                .Fetch(session => session.ProcessQuery(new GetTestItemQuery(expected.Id)))
                .AssertAndGet();

            Assert.Equal(expected, actual, TestItemComparer.Default);
        }

        [Fact(DisplayName = "Should execute command")]
        public async Task Command()
        {
            var expected = await CreateSessionFactory()
                .Execute(session => session.ExecuteCommandAndReturn(new AddTestItemCommand(TestItemGenerator.Generate())))
                .AssertAndGet();

            expected.Value = Guid.NewGuid().ToString();

            await CreateSessionFactory()
                .Execute(session => session.ExecuteCommand(new EditTestItemCommand(expected)))
                .AssertAndGet();

            var actual = await CreateSessionFactory()
                .Fetch(session => session.ProcessQuery(new GetTestItemQuery(expected.Id)))
                .AssertAndGet();

            Assert.Equal(expected, actual, TestItemComparer.Default);
        }

        [Fact(DisplayName = "Should publish notification")]
        public async Task Notification()
        {
            var expected = await CreateSessionFactory()
                .Execute(session => session.ExecuteCommandAndReturn(new AddTestItemCommand(TestItemGenerator.Generate())))
                .AssertAndGet();

            Assert.Contains(expected, CatchAddedTestItemsWhenAdded.CaughtItems, TestItemComparer.Default);
        }
    }
}
