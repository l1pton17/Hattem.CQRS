﻿using System;
using Hattem.CQRS.Commands.Pipeline;

namespace Hattem.CQRS.Commands
{
    public interface ICommandProcessorFactory<in TConnection>
        where TConnection : IHattemConnection
    {
        ICommandProcessor<TConnection> Create();
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

        public ICommandProcessor<TConnection> Create()
        {
            return new CommandProcessor<TSession, TConnection>(
                _handlerProvider,
                _commandExecutor);
        }
    }
}
