﻿using System;

namespace Hattem.CQRS.Commands
{
    public readonly struct CommandExecutionContext<TConnection, TCommand>
        where TConnection : IHattemConnection
        where TCommand : ICommand
    {
        public ICommandHandler<TConnection, TCommand> Handler { get; }

        public TConnection Connection { get; }

        public TCommand Command { get; }

        public CommandExecutionContext(
            TConnection connection,
            ICommandHandler<TConnection, TCommand> handler,
            TCommand command
        )
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }
    }

    public struct CommandWithReturnExecutionContext<TConnection, TReturn>
        where TConnection : IHattemConnection
    {
        public ICommandHandler<TConnection, ICommand<TReturn>, TReturn> Handler { get; }

        public TConnection Connection { get; }

        public ICommand<TReturn> Command { get; }

        public CommandWithReturnExecutionContext(
            ICommandHandler<TConnection, ICommand<TReturn>, TReturn> handler,
            TConnection connection,
            ICommand<TReturn> command
        )
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Connection = connection;
        }
    }

    public static class CommandExecutionContext
    {
        public static CommandWithReturnExecutionContext<TConnection, TReturn> CreateWithReturn<TConnection, TReturn>(
            ICommandHandler<TConnection, ICommand<TReturn>, TReturn> handler,
            TConnection connection,
            ICommand<TReturn> command
        )
            where TConnection : IHattemConnection
        {
            return new CommandWithReturnExecutionContext<TConnection, TReturn>(
                handler,
                connection,
                command);
        }

        public static CommandExecutionContext<TConnection, TCommand> Create<TConnection, TCommand>(
            TConnection connection,
            ICommandHandler<TConnection, TCommand> handler,
            TCommand command
        )
            where TConnection : IHattemConnection
            where TCommand : ICommand
        {
            return new CommandExecutionContext<TConnection, TCommand>(
                connection,
                handler,
                command);
        }
    }
}
