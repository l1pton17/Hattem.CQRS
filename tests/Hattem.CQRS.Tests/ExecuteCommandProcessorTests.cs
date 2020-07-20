using System.Collections.Generic;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Tests.Framework;
using Hattem.CQRS.Tests.Framework.Commands;
using Hattem.CQRS.Tests.Framework.Commands.Pipeline;
using Hattem.CQRS.Tests.Framework.Errors;
using Xunit;

namespace Hattem.CQRS.Tests
{
    [CategoryTrait("CommandProcessor Execute")]
    public sealed class ExecuteCommandProcessorTests : TestsBase
    {
        public static IEnumerable<object[]> ProcessTestsData
        {
            get
            {
                var (successCommand, successResponse) = CommandHandlerMock.GetCommand();

                yield return new object[] {successCommand, successResponse};

                var errorResponse = ApiResponse.Error(new TestError());
                var errorCommand = CommandHandlerMock.GetCommand(errorResponse);

                yield return new object[] {errorCommand, errorResponse};

                var (anotherSuccessCommand, anotherSuccessResponse) = CommandHandlerMock.GetCommand();

                yield return new object[] {anotherSuccessCommand, anotherSuccessResponse};

                var anotherErrorResponse = ApiResponse.Error(new TestError());
                var anotherErrorCommand = CommandHandlerMock.GetCommand(errorResponse);

                yield return new object[] {anotherErrorCommand, anotherErrorResponse};
            }
        }

        [Fact(DisplayName = "Should execute pipeline and pass connection in context")]
        public async Task ExecuteAndReturn_ExecutePipelineAndPassConnectionInContext()
        {
            var session = CreateSession();
            var (command, _) = CommandHandlerMock.GetCommand();

            await session.ExecuteCommand(command);

            CatchCommandPipelineStep.AssertCommandContextCaptured<CommandMock>(
                // ReSharper disable once IsExpressionAlwaysTrue
                context => context.Connection is HattemSessionMock);
        }

        [Fact(DisplayName = "Should execute pipeline and pass handler in context")]
        public async Task ExecuteAndReturn_ExecutePipelineAndPassHandlerInContext()
        {
            var session = CreateSession();
            var (command, _) = CommandHandlerMock.GetCommand();

            await session.ExecuteCommand(command);

            CatchCommandPipelineStep.AssertCommandContextCaptured<CommandMock>(
                context => context.Handler is CommandHandlerMock);
        }

        [Fact(DisplayName = "Should execute pipeline and pass command in context")]
        public async Task ExecuteAndReturn_ExecutePipelineAndPassCommandInContext()
        {
            var session = CreateSession();
            var (command, _) = CommandHandlerMock.GetCommand();

            await session.ExecuteCommand(command);

            CatchCommandPipelineStep.AssertCommandContextCaptured<CommandMock>(
                context => context.Command.Id == command.Id);
        }

        [Theory(DisplayName = "Should execute handler")]
        [MemberData(nameof(ProcessTestsData))]
        public async Task Execute_ExecuteHandler(dynamic command, ApiResponse<Unit> expected)
        {
            var session = CreateSession();

            var actual = await session.ExecuteCommand(command);

            Assert.Equal(actual.IsOk, expected.IsOk);
            Assert.Equal(actual.Error?.Code, expected.Error?.Code);
            Assert.Equal(actual.Data, expected.Data);
        }
    }
}
