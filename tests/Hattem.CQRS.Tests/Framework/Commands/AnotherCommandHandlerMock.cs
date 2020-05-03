using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;

namespace Hattem.CQRS.Tests.Framework.Commands
{
    public sealed class AnotherCommandMock : ICommand
    {
        public Guid Id { get; }

        private AnotherCommandMock(Guid id)
        {
            Id = id;
        }

        public static AnotherCommandMock New()
        {
            return new AnotherCommandMock(Guid.NewGuid());
        }
    }

    public sealed class AnotherCommandHandlerMock : ICommandHandlerMock<AnotherCommandMock>
    {
        private static readonly ConcurrentDictionary<Guid, ApiResponse<Unit>> _returns =
            new ConcurrentDictionary<Guid, ApiResponse<Unit>>();

        public Task<ApiResponse<Unit>> Execute(HattemSessionMock connection, AnotherCommandMock command)
        {
            var result = _returns[command.Id];

            return Task.FromResult(result);
        }

        public static (AnotherCommandMock Command, ApiResponse<Unit> Response) GetCommand()
        {
            var response = ApiResponse.Ok();
            var command = GetCommand(response);

            return (command, response);
        }

        public static AnotherCommandMock GetCommand(ApiResponse<Unit> response)
        {
            var command = AnotherCommandMock.New();

            _returns.AddOrUpdate(command.Id, response, (_, __) => response);

            return command;
        }
    }
}