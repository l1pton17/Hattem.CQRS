using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS
{
    public interface IHandlerProvider<in TSession, in TConnection>
        where TSession : IHattemSession
        where TConnection : IHattemConnection
    {
        IEnumerable<INotificationHandler<TSession, TNotification>> GetNotificationHandlers<TNotification>()
            where TNotification : INotification;

        ICommandHandler<TConnection, TCommand> GetCommandHandler<TCommand>()
            where TCommand : ICommand;

        ICommandHandler<TConnection, ICommand<TReturn>, TReturn> GetCommandWithReturnHandler<TReturn>(Type commandType);

        IQueryHandler<TConnection, IQuery<TResult>, TResult> GetQueryHandler<TResult>(Type queryType);
    }

    internal sealed class HandlerProvider<TSession, TConnection> : IHandlerProvider<TSession, TConnection>
        where TSession : IHattemSession
        where TConnection : IHattemConnection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Type, object> _commandHandlers;
        private readonly ConcurrentDictionary<Type, object> _commandWithReturnHandlers;
        private readonly ConcurrentDictionary<Type, object> _queryHandlers;
        private readonly ConcurrentDictionary<Type, IEnumerable<object>> _notificationHandlers;

        public HandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _commandHandlers = new ConcurrentDictionary<Type, object>();
            _commandWithReturnHandlers = new ConcurrentDictionary<Type, object>();
            _queryHandlers = new ConcurrentDictionary<Type, object>();
            _notificationHandlers = new ConcurrentDictionary<Type, IEnumerable<object>>();
        }

        public IEnumerable<INotificationHandler<TSession, TNotification>> GetNotificationHandlers<TNotification>()
            where TNotification : INotification
        {
            var key = typeof(TNotification);

            if (!_notificationHandlers.TryGetValue(key, out var handlers))
            {
                handlers = _notificationHandlers.GetOrAdd(
                    key,
                    _ => _serviceProvider
                        .GetServices(
                            typeof(INotificationHandler<,>)
                                .MakeGenericType(
                                    typeof(TSession),
                                    typeof(TNotification)))
                        .Cast<IHasNotificationHandlerOptions>()
                        .OrderBy(v => v.Options.Order)
                        .ToImmutableArray());
            }

            return (IEnumerable<INotificationHandler<TSession, TNotification>>) handlers;
        }

        public ICommandHandler<TConnection, TCommand> GetCommandHandler<TCommand>()
            where TCommand : ICommand
        {
            var key = typeof(TCommand);

            if (!_commandHandlers.TryGetValue(key, out var handler))
            {
                handler = _commandHandlers.GetOrAdd(
                    key,
                    _ =>
                    {
                        var commandHandlerType = typeof(ICommandHandler<,>)
                            .MakeGenericType(
                                typeof(TConnection),
                                typeof(TCommand));

                        return _serviceProvider.GetRequiredService(commandHandlerType);
                    });
            }

            return (ICommandHandler<TConnection, TCommand>) handler;
        }

        public ICommandHandler<TConnection, ICommand<TReturn>, TReturn> GetCommandWithReturnHandler<TReturn>(Type commandType)
        {
            var key = commandType;

            if (!_commandWithReturnHandlers.TryGetValue(key, out var handler))
            {
                handler = _commandWithReturnHandlers.GetOrAdd(
                    key,
                    _ =>
                    {
                        var discoveryType = typeof(CommandHandlerDiscovery<,,,>)
                            .MakeGenericType(
                                typeof(ICommandHandler<,,>)
                                    .MakeGenericType(
                                        typeof(TConnection),
                                        commandType,
                                        typeof(TReturn)
                                    ),
                                typeof(TConnection),
                                commandType,
                                typeof(TReturn));

                        return ActivatorUtilities.CreateInstance(_serviceProvider, discoveryType);
                    });
            }

            return (ICommandHandler<TConnection, ICommand<TReturn>, TReturn>) handler;
        }

        public IQueryHandler<TConnection, IQuery<TResult>, TResult> GetQueryHandler<TResult>(Type queryType)
        {
            var key = queryType;

            if (!_queryHandlers.TryGetValue(key, out var handler))
            {
                handler = _queryHandlers.GetOrAdd(
                    key,
                    _ =>
                    {
                        var discoveryType = typeof(QueryHandlerDiscovery<,,,>)
                            .MakeGenericType(
                                typeof(IQueryHandler<,,>)
                                    .MakeGenericType(
                                        typeof(TConnection),
                                        queryType,
                                        typeof(TResult)
                                    ),
                                typeof(TConnection),
                                queryType,
                                typeof(TResult));

                        return ActivatorUtilities.CreateInstance(_serviceProvider, discoveryType);
                    });
            }

            return (IQueryHandler<TConnection, IQuery<TResult>, TResult>) handler;
        }
    }
}