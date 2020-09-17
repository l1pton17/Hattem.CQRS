using System;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Containers;
using Hattem.CQRS.Extensions;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS
{
    public interface IHandlerAdapterFactory
    {
        IQueryHandler<TConnection, IQuery<TResult>, TResult> AdaptQueryHandler<TConnection, TResult>(IContainerBuilder builder, Type queryType)
            where TConnection : IHattemConnection;

        ICommandHandler<TConnection, ICommand<TReturn>, TReturn> AdaptCommandHandler<TConnection, TReturn>(IContainerBuilder builder, Type commandType)
            where TConnection : IHattemConnection;
    }

    internal sealed class HandlerAdapterFactory : IHandlerAdapterFactory
    {
        public IQueryHandler<TConnection, IQuery<TResult>, TResult> AdaptQueryHandler<TConnection, TResult>(IContainerBuilder builder, Type queryType)
            where TConnection : IHattemConnection
        {
            var queryHandlerType = typeof(IQueryHandler<,,>)
                .MakeGenericType(
                    typeof(TConnection),
                    queryType,
                    typeof(TResult)
                );

            var queryHandler = builder.GetRequiredService(queryHandlerType);

            var handlerType = typeof(QueryHandlerAdapter<,,,>);

            var discoveryType = handlerType
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

            return (IQueryHandler<TConnection, IQuery<TResult>, TResult>) discoveryType
                    .GetConstructor(new[] {queryHandlerType})
                    ?.Invoke(new[] {queryHandler})
                ?? throw new InvalidOperationException();
        }

        public ICommandHandler<TConnection, ICommand<TReturn>, TReturn> AdaptCommandHandler<TConnection, TReturn>(IContainerBuilder builder, Type commandType)
            where TConnection : IHattemConnection
        {
            var commandHandlerType = typeof(ICommandHandler<,,>)
                .MakeGenericType(
                    typeof(TConnection),
                    commandType,
                    typeof(TReturn)
                );

            var commandHandler = builder.GetRequiredService(commandHandlerType);
            var handlerType = typeof(CommandHandlerAdapter<,,,>);

            var discoveryType = handlerType
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

            return (ICommandHandler<TConnection, ICommand<TReturn>, TReturn>) discoveryType
                    .GetConstructor(new[] {commandHandlerType})
                    ?.Invoke(new[] {commandHandler})
                ?? throw new InvalidOperationException();
        }
    }
}

/*

                        var queryHandler = _builder.GetRequiredService(queryHandlerType);
                        var handlerType = typeof(QueryHandlerAdapter<,,,>);

                        var discoveryType = handlerType
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
                        return discoveryType
                                .GetConstructor(new[] {queryHandlerType})
                                ?.Invoke(new[] {queryHandler})
                            ?? throw new InvalidOperationException();*/
