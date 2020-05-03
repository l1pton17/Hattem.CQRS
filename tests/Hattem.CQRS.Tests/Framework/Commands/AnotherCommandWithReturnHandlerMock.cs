using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;

namespace Hattem.CQRS.Tests.Framework.Commands
{
    public sealed class AnotherCommandWithReturnMock : ICommand<CommandMockReturn>
    {
        public Guid Id { get; }

        private AnotherCommandWithReturnMock(Guid id)
        {
            Id = id;
        }

        public static AnotherCommandWithReturnMock New()
        {
            return new AnotherCommandWithReturnMock(Guid.NewGuid());
        }
    }

    public sealed class AnotherCommandWithReturnHandlerMock : ICommandHandlerMock<AnotherCommandWithReturnMock, CommandMockReturn>
    {
        private static readonly ConcurrentDictionary<Guid, ApiResponse<CommandMockReturn>> _returns =
            new ConcurrentDictionary<Guid, ApiResponse<CommandMockReturn>>();

        public Task<ApiResponse<CommandMockReturn>> Execute(HattemSessionMock connection, AnotherCommandWithReturnMock command)
        {
            var result = _returns[command.Id];

            return Task.FromResult(result);
        }

        public static (AnotherCommandWithReturnMock Command, ApiResponse<CommandMockReturn> Response) GetCommand()
        {
            var result = CommandMockReturn.New();
            var command = GetCommand(result);

            return (command, ApiResponse.Ok(result));
        }

        public static AnotherCommandWithReturnMock GetCommand(ApiResponse<CommandMockReturn> response)
        {
            var command = AnotherCommandWithReturnMock.New();

            _returns.AddOrUpdate(command.Id, response, (_, __) => response);

            return command;
        }

        private static AnotherCommandWithReturnMock GetCommand(CommandMockReturn result)
        {
            return GetCommand(ApiResponse.Ok(result));
        }
    }
}