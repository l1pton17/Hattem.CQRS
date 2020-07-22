using System;

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

    public readonly struct CommandWithReturnExecutionContext<TConnection, TCommand, TReturn>
        where TConnection : IHattemConnection
        where TCommand : ICommand<TReturn>
    {
        public ICommandHandler<TConnection, TCommand, TReturn> Handler { get; }

        public TConnection Connection { get; }

        public TCommand Command { get; }

        public CommandWithReturnExecutionContext(
            ICommandHandler<TConnection, TCommand, TReturn> handler,
            TConnection connection,
            TCommand command
        )
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Command = command;
        }
    }

    public static class CommandExecutionContext
    {
        public static CommandWithReturnExecutionContext<TConnection, TCommand, TReturn> CreateWithReturn<TConnection, TCommand, TReturn>(
            ICommandHandler<TConnection, TCommand, TReturn> handler,
            TConnection connection,
            TCommand command
        )
            where TConnection : IHattemConnection
            where TCommand : ICommand<TReturn>
        {
            return new CommandWithReturnExecutionContext<TConnection, TCommand, TReturn>(
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
