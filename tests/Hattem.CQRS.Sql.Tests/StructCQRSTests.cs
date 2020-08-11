using System;
using System.Threading.Tasks;
using Hattem.CQRS.Sql.Extensions;
using Hattem.CQRS.Sql.Tests.Framework;
using Hattem.CQRS.Sql.Tests.Framework.Commands.TestItems;
using Hattem.CQRS.Sql.Tests.Framework.Comparers;
using Hattem.CQRS.Sql.Tests.Framework.Extensions;
using Hattem.CQRS.Sql.Tests.Framework.Generators;
using Hattem.CQRS.Sql.Tests.Framework.Models;
using Hattem.CQRS.Sql.Tests.Framework.Notifications.Handlers.TestItems;
using Hattem.CQRS.Sql.Tests.Framework.Queries.TestItems;
using Xunit;

namespace Hattem.CQRS.Sql.Tests
{
    [CategoryTrait("CQRS structs")]
    public sealed class StructCQRSTests : TestsBase
    {
        public StructCQRSTests(TestFixture fixture)
            : base(fixture)
        {
        }

        [Fact(DisplayName = "Should execute command with return")]
        public async Task ExecuteCommandAndReturn()
        {
            await using var session = CreateSession();

            var expected = await session
                .ExecuteStructCommandAndReturn(
                    new AddTestItemStructCommand(TestItemGenerator.Generate()),
                    Returns<TestItem>.Type)
                .AssertAndGet();

            var actual = await session
                .ProcessStructQuery(
                    new GetTestItemStructQuery(expected.Id),
                    Returns<TestItem>.Type)
                .AssertAndGet();

            Assert.Equal(expected, actual, TestItemComparer.Default);

            await session.CommitAsync();
        }

        [Fact(DisplayName = "Should execute query")]
        public async Task Query()
        {
            var expected = await CreateSessionFactory()
                .Execute(
                    session => session.ExecuteStructCommandAndReturn(
                        new AddTestItemStructCommand(TestItemGenerator.Generate()),
                        Returns<TestItem>.Type))
                .AssertAndGet();

            var actual = await CreateSessionFactory()
                .Fetch(
                    session => session.ProcessStructQuery(
                        new GetTestItemStructQuery(expected.Id),
                        Returns<TestItem>.Type))
                .AssertAndGet();

            Assert.Equal(expected, actual, TestItemComparer.Default);
        }

        [Fact(DisplayName = "Should execute command")]
        public async Task Command()
        {
            var expected = await CreateSessionFactory()
                .Execute(
                    session => session.ExecuteStructCommandAndReturn(
                        new AddTestItemStructCommand(TestItemGenerator.Generate()),
                        Returns<TestItem>.Type))
                .AssertAndGet();

            expected.Value = Guid.NewGuid().ToString();

            await CreateSessionFactory()
                .Execute(session => session.ExecuteCommand(new EditTestItemStructCommand(expected)))
                .AssertAndGet();

            var actual = await CreateSessionFactory()
                .Fetch(
                    session => session.ProcessStructQuery(
                        new GetTestItemStructQuery(expected.Id),
                        Returns<TestItem>.Type))
                .AssertAndGet();

            Assert.Equal(expected, actual, TestItemComparer.Default);
        }

        [Fact(DisplayName = "Should publish notification")]
        public async Task Notification()
        {
            var expected = await CreateSessionFactory()
                .Execute(
                    session => session.ExecuteStructCommandAndReturn(
                        new AddTestItemStructCommand(TestItemGenerator.Generate()),
                        Returns<TestItem>.Type))
                .AssertAndGet();

            Assert.Contains(expected, CatchAddedTestItemsWhenAdded.CaughtItems, TestItemComparer.Default);
        }
    }
}
