using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Commands.Pipeline
{
    public interface ICommandPipelineStep
    {
        Task<ApiResponse<Unit>> Execute<TConnection, TCommand>(
            CommandExecutionContext<TConnection, TCommand> context)
            where TConnection : IHattemConnection
            where TCommand : ICommand;

        Task<ApiResponse<TReturn>> ExecuteWithReturn<TConnection, TReturn>(
            CommandWithReturnExecutionContext<TConnection, TReturn> context)
            where TConnection : IHattemConnection;
    }
}
