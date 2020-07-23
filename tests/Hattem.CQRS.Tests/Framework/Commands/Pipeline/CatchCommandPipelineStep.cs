using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Commands.Pipeline;
using Xunit;

namespace Hattem.CQRS.Tests.Framework.Commands.Pipeline
{
    public sealed class CatchCommandPipelineStep : ICommandPipelineStep
    {
        public Task<ApiResponse<Unit>> Execute<TConnection, TCommand>(
            Func<CommandExecutionContext<TConnection, TCommand>, Task<ApiResponse<Unit>>> next,
            CommandExecutionContext<TConnection, TCommand> context
        )
            where TConnection : IHattemConnection
            where TCommand : ICommand
        {
            CommandCapturedContextStorage<TConnection, TCommand>.CapturedContext.Add(context);

            return next(context);
        }

        public Task<ApiResponse<TReturn>> ExecuteWithReturn<TConnection, TCommand, TReturn>(
            Func<CommandWithReturnExecutionContext<TConnection, TCommand, TReturn>, Task<ApiResponse<TReturn>>> next,
            CommandWithReturnExecutionContext<TConnection, TCommand, TReturn> context
        )
            where TConnection : IHattemConnection
            where TCommand : ICommand<TReturn>
        {
            CommandWithReturnCapturedContextStorage<TConnection, TCommand, TReturn>.CapturedContext.Add(context);

            return next(context);
        }

        public static void AssertCommandContextCaptured<TCommand>(Func<CommandExecutionContext<HattemSessionMock, TCommand>, bool> predicate)
            where TCommand : ICommand
        {
            var contains = CommandCapturedContextStorage<HattemSessionMock, TCommand>.CapturedContext.Any(predicate);

            Assert.True(contains);
        }

        public static void AssertCommandWithReturnContextCaptured<TReturn>(Func<CommandWithReturnExecutionContext<HattemSessionMock, ICommand<TReturn>, TReturn>, bool> predicate)
        {
            var contains = CommandWithReturnCapturedContextStorage<HattemSessionMock, ICommand<TReturn>, TReturn>.CapturedContext.Any(predicate);

            Assert.True(contains);
        }

        public static void AssertCommandWithReturnContextCaptured<TCommand, TReturn>(Func<CommandWithReturnExecutionContext<HattemSessionMock, TCommand, TReturn>, bool> predicate)
            where TCommand : ICommand<TReturn>
        {
            var contains = CommandWithReturnCapturedContextStorage<HattemSessionMock, TCommand, TReturn>.CapturedContext.Any(predicate);

            Assert.True(contains);
        }

        private static class CommandCapturedContextStorage<TConnection, TCommand>
            where TConnection : IHattemConnection
            where TCommand : ICommand
        {
            public static ConcurrentBag<CommandExecutionContext<TConnection, TCommand>> CapturedContext { get; } =
                new ConcurrentBag<CommandExecutionContext<TConnection, TCommand>>();
        }

        private static class CommandWithReturnCapturedContextStorage<TConnection, TCommand, TReturn>
            where TConnection : IHattemConnection
            where TCommand : ICommand<TReturn>
        {
            public static ConcurrentBag<CommandWithReturnExecutionContext<TConnection, TCommand, TReturn>> CapturedContext { get; } =
                new ConcurrentBag<CommandWithReturnExecutionContext<TConnection, TCommand, TReturn>>();
        }
    }
}
