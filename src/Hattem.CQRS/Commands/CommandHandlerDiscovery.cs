using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Extensions;

namespace Hattem.CQRS.Commands
{
    internal sealed class CommandHandlerDiscovery<THandler, TConnection, TCommand, TReturn> :
        ICommandHandler<TConnection, ICommand<TReturn>, TReturn>,
        IHasHandlerName
        where THandler : class, ICommandHandler<TConnection, TCommand, TReturn>
        where TConnection : IHattemConnection
        where TCommand : class, ICommand<TReturn>
    {
        private readonly THandler _handler;

        public string Name { get; }

        public CommandHandlerDiscovery(THandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));

            Name = _handler.GetType().GetFriendlyName();
        }

        public Task<ApiResponse<TReturn>> Execute(TConnection connection, ICommand<TReturn> command)
        {
            return _handler.Execute(connection, (TCommand) command);
        }
    }

    internal sealed class CommandHandlerWithCacheInvalidationDiscovery<THandler, TConnection, TCommand, TReturn> :
        ICommandHandler<TConnection, ICommand<TReturn>, TReturn>,
        IInvalidateCacheCommandHandler<ICommand<TReturn>>,
        IHasHandlerName
        where THandler :
        class,
        ICommandHandler<TConnection, TCommand, TReturn>,
        IInvalidateCacheCommandHandler<TCommand>
        where TConnection : IHattemConnection
        where TCommand : class, ICommand<TReturn>
    {
        private readonly THandler _handler;

        public string Name { get; }

        public CommandHandlerWithCacheInvalidationDiscovery(THandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));

            Name = _handler.GetType().GetFriendlyName();
        }

        public Task<ApiResponse<TReturn>> Execute(TConnection connection, ICommand<TReturn> command)
        {
            return _handler.Execute(connection, (TCommand) command);
        }

        public IEnumerable<(string Key, string Region)> GetCacheKeys(ICommand<TReturn> command)
        {
            return _handler.GetCacheKeys((TCommand) command);
        }

        public IEnumerable<string> GetCacheRegions(ICommand<TReturn> command)
        {
            return _handler.GetCacheRegions((TCommand) command);
        }
    }
}
