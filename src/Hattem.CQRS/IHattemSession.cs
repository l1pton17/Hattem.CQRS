using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS
{
    public interface IHattemSession
    {
        Task<ApiResponse<TResult>> ProcessQuery<TResult>(IQuery<TResult> query);

        Task<ApiResponse<TResult>> ProcessStructQuery<TQuery, TResult>(in TQuery query, Returns<TResult> returns)
            where TQuery : struct, IQuery<TResult>;

        Task<ApiResponse<Unit>> ExecuteCommand<TCommand>(TCommand command)
            where TCommand : ICommand;

        Task<ApiResponse<TReturn>> ExecuteCommandAndReturn<TReturn>(ICommand<TReturn> command);

        Task<ApiResponse<TReturn>> ExecuteStructCommandAndReturn<TCommand, TReturn>(in TCommand command, Returns<TReturn> returns)
            where TCommand : struct, ICommand<TReturn>;
    }
}
