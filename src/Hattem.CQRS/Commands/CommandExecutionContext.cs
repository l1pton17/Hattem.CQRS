using System;

namespace Hattem.CQRS.Commands
{
    public struct CommandExecutionContext<TConnection, TCommand>
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
            Command = command;
            Connection = connection;
        }
    }

    public struct CommandWithReturnExecutionContext<TConnection, TReturn>
        where TConnection : IHattemConnection
    {
        public ICommandHandler<TConnection, ICommand<TReturn>, TReturn> Handler { get; }

        public TConnection Connection { get; }

        public ICommand<TReturn> Command { get; }

        public CommandWithReturnExecutionContext(
            TConnection connection,
            ICommandHandler<TConnection, ICommand<TReturn>, TReturn> handler,
            ICommand<TReturn> command
        )
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Command = command;
            Connection = connection;
        }
    }

    public static class CommandExecutionContext
    {
        public static CommandWithReturnExecutionContext<TConnection, TReturn> CreateWithReturn<TConnection, TReturn>(
            TConnection connection,
            ICommandHandler<TConnection, ICommand<TReturn>, TReturn> handler,
            ICommand<TReturn> command
        )
            where TConnection : IHattemConnection
        {
            return new CommandWithReturnExecutionContext<TConnection, TReturn>(
                connection,
                handler,
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