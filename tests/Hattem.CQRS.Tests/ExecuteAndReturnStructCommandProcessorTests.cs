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
    [CategoryTrait("StructCommandProcessor ExecuteAndReturn")]
    public sealed class ExecuteAndReturnStructCommandProcessorTests : TestsBase
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
            var (command, _) = StructCommandWithReturnHandlerMock.GetCommand();

            await session.ExecuteStructCommandAndReturn(command, Returns<CommandMockReturn>.Type);

            CatchCommandPipelineStep.AssertCommandWithReturnContextCaptured<StructCommandWithReturnMock, CommandMockReturn>(
                // ReSharper disable once IsExpressionAlwaysTrue
                context => context.Connection is HattemSessionMock);
        }

        [Fact(DisplayName = "Should execute pipeline and pass handler in context")]
        public async Task ExecuteAndReturn_ExecutePipelineAndPassHandlerInContext()
        {
            var session = CreateSession();
            var (command, _) = StructCommandWithReturnHandlerMock.GetCommand();

            await session.ExecuteStructCommandAndReturn(command, Returns<CommandMockReturn>.Type);

            CatchCommandPipelineStep.AssertCommandWithReturnContextCaptured<StructCommandWithReturnMock, CommandMockReturn>(
                context => context.Handler is StructCommandWithReturnHandlerMock);
        }

        [Fact(DisplayName = "Should execute pipeline and pass command in context")]
        public async Task ExecuteAndReturn_ExecutePipelineAndPassCommandInContext()
        {
            var session = CreateSession();
            var (command, _) = StructCommandWithReturnHandlerMock.GetCommand();

            await session.ExecuteStructCommandAndReturn(command, Returns<CommandMockReturn>.Type);

            CatchCommandPipelineStep.AssertCommandWithReturnContextCaptured<StructCommandWithReturnMock, CommandMockReturn>(
                context => context.Command.Equals(command));
        }

        [Fact(DisplayName = "Should return successful response from handler")]
        public async Task  ExecuteAndReturn_SuccessfulResponse_ReturnsResponseFromHandler()
        {
            var session = CreateSession();
            var (command, expectedResponse) = StructCommandWithReturnHandlerMock.GetCommand();

            var actualResponse = await session.ExecuteStructCommandAndReturn(command, Returns<CommandMockReturn>.Type);

            Assert.Equal(expectedResponse, actualResponse);
        }

        [Fact(DisplayName = "Should return error response from handler")]
        public async Task  ExecuteAndReturn_ErrorResponse_ReturnsResponseFromHandler()
        {
            var session = CreateSession();
            var expectedResponse = ApiResponse.Error<CommandMockReturn>(new TestError());
            var command = StructCommandWithReturnHandlerMock.GetCommand(expectedResponse);

            var actualResponse = await session.ExecuteStructCommandAndReturn(command, Returns<CommandMockReturn>.Type);

            Assert.Equal(expectedResponse, actualResponse);
        }

        [Fact(DisplayName = "Should return successful response from multiple handler")]
        public async Task  ExecuteAndReturn_SuccessfulResponseFromMultipleHandlers_ReturnsResponseFromHandler()
        {
            var session = CreateSession();
            var (command, expectedResponse) = StructCommandWithReturnHandlerMock.GetCommand();

            var actualResponse = await session.ExecuteStructCommandAndReturn(command, Returns<CommandMockReturn>.Type);

            Assert.Equal(expectedResponse, actualResponse);

            var (anotherCommand, anotherExpectedResponse) = AnotherStructCommandWithReturnHandlerMock.GetCommand();

            var anotherActualResponse = await session.ExecuteStructCommandAndReturn(anotherCommand, Returns<CommandMockReturn>.Type);

            Assert.Equal(anotherExpectedResponse, anotherActualResponse);
        }
    }
}
