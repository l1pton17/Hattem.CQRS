using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS
{
    public interface IHattemSession
    {
        Task<ApiResponse<TResult>> ProcessQuery<TResult>(IQuery<TResult> query);

        Task<ApiResponse<Unit>> ExecuteCommand<TCommand>(TCommand command)
            where TCommand : ICommand;

        Task<ApiResponse<TReturn>> ExecuteCommandAndReturn<TReturn>(ICommand<TReturn> command);
    }
}
