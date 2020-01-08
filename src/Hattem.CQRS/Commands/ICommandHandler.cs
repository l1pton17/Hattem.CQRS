using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Commands
{
    /// <summary>
    /// Base interface for command handlers. Do not use explicit.
    /// </summary>
    /// <typeparam name="TConnection"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandHandler<in TConnection, in TCommand>
        where TCommand : ICommand
        where TConnection : IHattemConnection
    {
        Task<ApiResponse<Unit>> Execute(TConnection connection, TCommand command);
    }

    /// <summary>
    /// Base interface for command handlers
    /// </summary>
    /// <typeparam name="TConnection"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    public interface ICommandHandler<in TConnection, in TCommand, TReturn>
        where TCommand : ICommand<TReturn>
        where TConnection : IHattemConnection
    {
        Task<ApiResponse<TReturn>> Execute(TConnection connection, TCommand command);
    }
}
