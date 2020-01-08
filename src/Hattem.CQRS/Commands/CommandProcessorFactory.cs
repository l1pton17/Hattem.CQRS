using System;
using Microsoft.Extensions.Logging;

namespace Hattem.CQRS.Commands
{
    public interface ICommandProcessorFactory<in TConnection>
        where TConnection : IHattemConnection
    {
        ICommandProcessor Create(TConnection connection);
    }

    internal sealed class CommandProcessorFactory<TSession, TConnection> : ICommandProcessorFactory<TConnection>
        where TConnection : IHattemConnection
        where TSession : IHattemSession
    {
        private readonly IHandlerProvider<TSession, TConnection> _handlerProvider;
        private readonly ILoggerFactory _loggerFactory;

        public CommandProcessorFactory(
            IHandlerProvider<TSession, TConnection> handlerProvider,
            ILoggerFactory loggerFactory
        )
        {
            _handlerProvider = handlerProvider ?? throw new ArgumentNullException(nameof(handlerProvider));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public ICommandProcessor Create(TConnection connection)
        {
            return new CommandProcessor<TSession, TConnection>(
                connection,
                _handlerProvider,
                _loggerFactory
            );
        }
    }
}