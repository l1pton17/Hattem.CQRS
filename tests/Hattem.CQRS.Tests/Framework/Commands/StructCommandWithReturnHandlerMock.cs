using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;

namespace Hattem.CQRS.Tests.Framework.Commands
{
    public readonly struct StructCommandWithReturnMock : ICommand<CommandMockReturn>
    {
        public Guid Id { get; }

        private StructCommandWithReturnMock(Guid id)
        {
            Id = id;
        }

        public static StructCommandWithReturnMock New()
        {
            return new StructCommandWithReturnMock(Guid.NewGuid());
        }
    }

    public sealed class StructCommandWithReturnHandlerMock : ICommandHandlerMock<StructCommandWithReturnMock, CommandMockReturn>
    {
        private static readonly ConcurrentDictionary<Guid, ApiResponse<CommandMockReturn>> _returns =
            new ConcurrentDictionary<Guid, ApiResponse<CommandMockReturn>>();

        public Task<ApiResponse<CommandMockReturn>> Execute(HattemSessionMock connection, StructCommandWithReturnMock command)
        {
            var result = _returns[command.Id];

            return Task.FromResult(result);
        }

        public static (StructCommandWithReturnMock Command, ApiResponse<CommandMockReturn> Response) GetCommand()
        {
            var result = CommandMockReturn.New();
            var command = GetCommand(result);

            return (command, ApiResponse.Ok(result));
        }

        public static StructCommandWithReturnMock GetCommand(ApiResponse<CommandMockReturn> response)
        {
            var command = StructCommandWithReturnMock.New();

            _returns.AddOrUpdate(command.Id, response, (_, __) => response);

            return command;
        }

        private static StructCommandWithReturnMock GetCommand(CommandMockReturn result)
        {
            return GetCommand(ApiResponse.Ok(result));
        }
    }
}
