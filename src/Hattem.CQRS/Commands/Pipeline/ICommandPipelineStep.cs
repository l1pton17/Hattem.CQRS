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

        Task<ApiResponse<TReturn>> ExecuteWithReturn<TConnection, TReturn>(
            Func<CommandWithReturnExecutionContext<TConnection, TReturn>, Task<ApiResponse<TReturn>>> next,
            CommandWithReturnExecutionContext<TConnection, TReturn> context)
            where TConnection : IHattemConnection;
    }
}
