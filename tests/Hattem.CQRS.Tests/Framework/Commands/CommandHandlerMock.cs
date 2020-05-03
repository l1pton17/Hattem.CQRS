using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;

namespace Hattem.CQRS.Tests.Framework.Commands
{
    public sealed class CommandMock : ICommand
    {
        public Guid Id { get; }

        private CommandMock(Guid id)
        {
            Id = id;
        }

        public static CommandMock New()
        {
            return new CommandMock(Guid.NewGuid());
        }
    }

    public sealed class CommandHandlerMock : ICommandHandlerMock<CommandMock>
    {
        private static readonly ConcurrentDictionary<Guid, ApiResponse<Unit>> _returns =
            new ConcurrentDictionary<Guid, ApiResponse<Unit>>();

        public Task<ApiResponse<Unit>> Execute(HattemSessionMock connection, CommandMock command)
        {
            var result = _returns[command.Id];

            return Task.FromResult(result);
        }

        public static (CommandMock Command, ApiResponse<Unit> Response) GetCommand()
        {
            var response = ApiResponse.Ok();
            var command = GetCommand(response);

            return (command, response);
        }

        public static CommandMock GetCommand(ApiResponse<Unit> response)
        {
            var command = CommandMock.New();

            _returns.AddOrUpdate(command.Id, response, (_, __) => response);

            return command;
        }
    }
}