using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands.Pipeline;

namespace Hattem.CQRS.Commands
{
    /// <summary>
    /// Command processor
    /// </summary>
    public interface ICommandProcessor<in TConnection>
        where TConnection : IHattemConnection
    {
        /// <summary>
        /// Execute a command
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="connection">Connection</param>
        /// <param name="command">Command</param>
        /// <returns></returns>
        Task<ApiResponse<Unit>> Execute<TCommand>(TConnection connection, TCommand command)
            where TCommand : ICommand;

        /// <summary>
        /// Execute a command with result
        /// </summary>
        /// <typeparam name="TReturn">Type of result</typeparam>
        /// <param name="connection">Connection</param>
        /// <param name="command">Command</param>
        /// <returns></returns>
        Task<ApiResponse<TReturn>> ExecuteAndReturn<TReturn>(TConnection connection, ICommand<TReturn> command);
    }

    internal sealed class CommandProcessor<TSession, TConnection> : ICommandProcessor<TConnection>
        where TConnection : IHattemConnection
        where TSession : IHattemSession
    {
        private readonly IHandlerProvider<TSession, TConnection> _handlerProvider;
        private readonly ICommandExecutor<TConnection> _executor;

        public CommandProcessor(
            IHandlerProvider<TSession, TConnection> handlerProvider,
            ICommandExecutor<TConnection> executor
        )
        {
            _handlerProvider = handlerProvider ?? throw new ArgumentNullException(nameof(handlerProvider));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<ApiResponse<Unit>> Execute<TCommand>(TConnection connection, TCommand command)
            where TCommand : ICommand
        {
            var commandHandler = _handlerProvider.GetCommandHandler<TCommand>();

            var context = CommandExecutionContext.Create(
                connection,
                commandHandler,
                command);

            return _executor.Execute(context);
        }

        public Task<ApiResponse<TReturn>> ExecuteAndReturn<TReturn>(TConnection connection, ICommand<TReturn> command)
        {
            var commandHandler = _handlerProvider.GetCommandWithReturnHandler<TReturn>(command.GetType());

            var context = CommandExecutionContext.CreateWithReturn(
                commandHandler,
                connection,
                command);

            return _executor.ExecuteWithReturn(context);
        }
    }
}
