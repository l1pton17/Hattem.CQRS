using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Commands.Pipeline
{
    public interface ICommandExecutor<TConnection>
        where TConnection : IHattemConnection
    {
        Task<ApiResponse<Unit>> Execute<TCommand>(
            CommandExecutionContext<TConnection, TCommand> context
        )
            where TCommand : ICommand;

        Task<ApiResponse<TReturn>> ExecuteWithReturn<TReturn>(
            CommandWithReturnExecutionContext<TConnection, TReturn> context
        );
    }

    internal sealed class CommandExecutor<TConnection> : ICommandExecutor<TConnection>
        where TConnection : IHattemConnection
    {
        private readonly ImmutableArray<ICommandPipelineStep> _steps;

        public CommandExecutor(IEnumerable<ICommandPipelineStep> steps)
        {
            _steps = steps.ToImmutableArray();
        }

        public Task<ApiResponse<Unit>> Execute<TCommand>(
            CommandExecutionContext<TConnection, TCommand> context
        )
            where TCommand : ICommand
        {
            for (var i = 0; i < _steps.Length; i++)
            {
                var current = _steps[i];
                var next = i + 1 < _steps.Length ? _steps[i + 1] : null;
            }
            throw new System.NotImplementedException();
        }

        public Task<ApiResponse<TReturn>> ExecuteWithReturn<TReturn>(
            CommandWithReturnExecutionContext<TConnection, TReturn> context
        )
        {
            throw new System.NotImplementedException();
        }
    }
}