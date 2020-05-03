using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;

namespace Hattem.CQRS.Tests.Framework.Commands
{
    public sealed class CommandWithReturnMock : ICommand<CommandMockReturn>
    {
        public Guid Id { get; }

        private CommandWithReturnMock(Guid id)
        {
            Id = id;
        }

        public static CommandWithReturnMock New()
        {
            return new CommandWithReturnMock(Guid.NewGuid());
        }
    }

    public sealed class CommandWithReturnHandlerMock : ICommandHandlerMock<CommandWithReturnMock, CommandMockReturn>
    {
        private static readonly ConcurrentDictionary<Guid, ApiResponse<CommandMockReturn>> _returns =
            new ConcurrentDictionary<Guid, ApiResponse<CommandMockReturn>>();

        public Task<ApiResponse<CommandMockReturn>> Execute(HattemSessionMock connection, CommandWithReturnMock command)
        {
            var result = _returns[command.Id];

            return Task.FromResult(result);
        }

        public static (CommandWithReturnMock Command, ApiResponse<CommandMockReturn> Response) GetCommand()
        {
            var result = CommandMockReturn.New();
            var command = GetCommand(result);

            return (command, ApiResponse.Ok(result));
        }

        public static CommandWithReturnMock GetCommand(ApiResponse<CommandMockReturn> response)
        {
            var command = CommandWithReturnMock.New();

            _returns.AddOrUpdate(command.Id, response, (_, __) => response);

            return command;
        }

        private static CommandWithReturnMock GetCommand(CommandMockReturn result)
        {
            return GetCommand(ApiResponse.Ok(result));
        }
    }
}