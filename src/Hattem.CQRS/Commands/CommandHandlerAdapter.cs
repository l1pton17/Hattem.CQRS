using System;
using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Commands
{
    internal sealed class CommandHandlerAdapter<THandler, TConnection, TCommand, TReturn> :
        ICommandHandler<TConnection, ICommand<TReturn>, TReturn>
        where THandler : class, ICommandHandler<TConnection, TCommand, TReturn>
        where TConnection : IHattemConnection
        where TCommand : class, ICommand<TReturn>
    {
        private readonly THandler _handler;

        public CommandHandlerAdapter(THandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task<ApiResponse<TReturn>> Execute(TConnection connection, ICommand<TReturn> command)
        {
            return _handler.Execute(connection, (TCommand) command);
        }
    }
}
