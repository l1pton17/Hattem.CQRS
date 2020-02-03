using System;
using Hattem.CQRS.Commands.Pipeline;

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
        private readonly ICommandExecutor<TConnection> _commandExecutor;

        public CommandProcessorFactory(
            IHandlerProvider<TSession, TConnection> handlerProvider,
            ICommandExecutor<TConnection> commandExecutor
        )
        {
            _handlerProvider = handlerProvider ?? throw new ArgumentNullException(nameof(handlerProvider));
            _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        }

        public ICommandProcessor Create(TConnection connection)
        {
            return new CommandProcessor<TSession, TConnection>(
                connection,
                _handlerProvider,
                _commandExecutor
            );
        }
    }
}