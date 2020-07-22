using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;

namespace Hattem.CQRS.Tests.Framework.Commands
{
    public readonly struct AnotherStructCommandWithReturnMock : ICommand<CommandMockReturn>
    {
        public Guid Id { get; }

        private AnotherStructCommandWithReturnMock(Guid id)
        {
            Id = id;
        }

        public static AnotherStructCommandWithReturnMock New()
        {
            return new AnotherStructCommandWithReturnMock(Guid.NewGuid());
        }
    }

    public sealed class AnotherStructCommandWithReturnHandlerMock : ICommandHandlerMock<AnotherStructCommandWithReturnMock, CommandMockReturn>
    {
        private static readonly ConcurrentDictionary<Guid, ApiResponse<CommandMockReturn>> _returns =
            new ConcurrentDictionary<Guid, ApiResponse<CommandMockReturn>>();

        public Task<ApiResponse<CommandMockReturn>> Execute(HattemSessionMock connection, AnotherStructCommandWithReturnMock command)
        {
            var result = _returns[command.Id];

            return Task.FromResult(result);
        }

        public static (AnotherStructCommandWithReturnMock Command, ApiResponse<CommandMockReturn> Response) GetCommand()
        {
            var result = CommandMockReturn.New();
            var command = GetCommand(result);

            return (command, ApiResponse.Ok(result));
        }

        public static AnotherStructCommandWithReturnMock GetCommand(ApiResponse<CommandMockReturn> response)
        {
            var command = AnotherStructCommandWithReturnMock.New();

            _returns.AddOrUpdate(command.Id, response, (_, __) => response);

            return command;
        }

        private static AnotherStructCommandWithReturnMock GetCommand(CommandMockReturn result)
        {
            return GetCommand(ApiResponse.Ok(result));
        }
    }
}
