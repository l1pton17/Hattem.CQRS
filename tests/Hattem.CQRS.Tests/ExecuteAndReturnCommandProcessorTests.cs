using System.Collections.Generic;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Extensions;
using Hattem.CQRS.Tests.Framework;
using Hattem.CQRS.Tests.Framework.Commands;
using Hattem.CQRS.Tests.Framework.Commands.Pipeline;
using Hattem.CQRS.Tests.Framework.Errors;
using Xunit;

namespace Hattem.CQRS.Tests
{
    [CategoryTrait("CommandProcessor ExecuteAndReturn")]
    public sealed class ExecuteAndReturnCommandProcessorTests : TestsBase
    {
        public static IEnumerable<object[]> ProcessTestsData
        {
            get
            {
                var (successCommand, successResponse) = CommandWithReturnHandlerMock.GetCommand();

                yield return new object[] {successCommand, successResponse};

                var errorResponse = ApiResponse.Error<CommandMockReturn>(new TestError());
                var errorQuery = CommandWithReturnHandlerMock.GetCommand(errorResponse);

                yield return new object[] {errorQuery, errorResponse};

                var (anotherSuccessQuery, anotherSuccessResponse) = CommandWithReturnHandlerMock.GetCommand();

                yield return new object[] {anotherSuccessQuery, anotherSuccessResponse};

                var anotherErrorResponse = ApiResponse.Error<CommandMockReturn>(new TestError());
                var anotherErrorQuery = CommandWithReturnHandlerMock.GetCommand(errorResponse);

                yield return new object[] {anotherErrorQuery, anotherErrorResponse};
            }
        }

        [Fact(DisplayName = "Should execute pipeline and pass connection in context")]
        public async Task ExecuteAndReturn_ExecutePipelineAndPassConnectionInContext()
        {
            var session = CreateSession();
            var (command, _) = CommandWithReturnHandlerMock.GetCommand();

            await session.ExecuteCommandAndReturn(command);

            CatchCommandPipelineStep.AssertCommandWithReturnContextCaptured<CommandMockReturn>(
                // ReSharper disable once IsExpressionAlwaysTrue
                context => context.Connection is HattemSessionMock);
        }

        [Fact(DisplayName = "Should execute pipeline and pass handler in context")]
        public async Task ExecuteAndReturn_ExecutePipelineAndPassHandlerInContext()
        {
            var session = CreateSession();
            var (command, _) = CommandWithReturnHandlerMock.GetCommand();

            await session.ExecuteCommandAndReturn(command);

            CatchCommandPipelineStep.AssertCommandWithReturnContextCaptured<CommandMockReturn>(
                context => context.Handler is CommandHandlerAdapter<ICommandHandler<HattemSessionMock, CommandWithReturnMock, CommandMockReturn>, HattemSessionMock, CommandWithReturnMock, CommandMockReturn>);
        }

        [Fact(DisplayName = "Should execute pipeline and pass command in context")]
        public async Task ExecuteAndReturn_ExecutePipelineAndPassCommandInContext()
        {
            var session = CreateSession();
            var (command, _) = CommandWithReturnHandlerMock.GetCommand();

            await session.ExecuteCommandAndReturn(command);

            CatchCommandPipelineStep.AssertCommandWithReturnContextCaptured<CommandMockReturn>(
                context => context.Command is CommandWithReturnMock actualCommand && actualCommand.Id == command.Id);
        }

        [Theory(DisplayName = "Should execute handler")]
        [MemberData(nameof(ProcessTestsData))]
        public async Task ExecuteAndReturn_ExecuteHandler(ICommand<CommandMockReturn> command, ApiResponse<CommandMockReturn> expected)
        {
            var session = CreateSession();

            var actual = await session.ExecuteCommandAndReturn(command);

            Assert.Equal(actual.IsOk, expected.IsOk);
            Assert.Equal(actual.Error?.Code, expected.Error?.Code);
            Assert.Equal(actual.Data, expected.Data);
        }
    }
}
