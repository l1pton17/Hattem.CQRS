using System;
using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Commands.Pipeline
{
    public interface ICommandPipelineStep
    {
        Task<ApiResponse<Unit>> Execute<TConnection, TCommand>(
            Func<CommandExecutionContext<TConnection, TCommand>, Task<ApiResponse<Unit>>> next,
            CommandExecutionContext<TConnection, TCommand> context)
            where TConnection : IHattemConnection
            where TCommand : ICommand;

        Task<ApiResponse<TReturn>> ExecuteWithReturn<TConnection, TCommand, TReturn>(
            Func<CommandWithReturnExecutionContext<TConnection, TCommand, TReturn>, Task<ApiResponse<TReturn>>> next,
            CommandWithReturnExecutionContext<TConnection, TCommand, TReturn> context)
            where TConnection : IHattemConnection
            where TCommand : ICommand<TReturn>;
    }
}
