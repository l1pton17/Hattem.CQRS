using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Commands.Pipeline
{
    public interface ICommandExecutor<TConnection>
        where TConnection : IHattemConnection
    {
        Task<ApiResponse<Unit>> Execute<TCommand>(
            CommandExecutionContext<TConnection, TCommand> context
        )
            where TCommand : ICommand;

        Task<ApiResponse<TReturn>> ExecuteWithReturn<TReturn>(
            CommandWithReturnExecutionContext<TConnection, TReturn> context
        );
    }

    internal sealed class CommandExecutor<TConnection> : ICommandExecutor<TConnection>
        where TConnection : IHattemConnection
    {
        private readonly ImmutableArray<ICommandPipelineStep> _steps;

        public CommandExecutor(IEnumerable<ICommandPipelineStep> steps)
        {
            _steps = steps.ToImmutableArray();
        }

        public Task<ApiResponse<Unit>> Execute<TCommand>(
            CommandExecutionContext<TConnection, TCommand> context
        )
            where TCommand : ICommand
        {
            ExecuteCache<TCommand>.EnsureInitialized(_steps);

            return ExecuteCache<TCommand>.Pipeline(context);
        }

        public Task<ApiResponse<TReturn>> ExecuteWithReturn<TReturn>(
            CommandWithReturnExecutionContext<TConnection, TReturn> context
        )
        {
            var pipeline = ExecuteWithReturnCache<TReturn>.GetPipeline(_steps, context.Command.GetType());

            return pipeline(context);
        }

        private static class ExecuteWithReturnCache<TReturn>
        {
            private static readonly ConcurrentDictionary<Type, Func<CommandWithReturnExecutionContext<TConnection, TReturn>, Task<ApiResponse<TReturn>>>> _cache =
                new ConcurrentDictionary<Type, Func<CommandWithReturnExecutionContext<TConnection, TReturn>, Task<ApiResponse<TReturn>>>>();

            public static Func<CommandWithReturnExecutionContext<TConnection, TReturn>, Task<ApiResponse<TReturn>>> GetPipeline(
                ImmutableArray<ICommandPipelineStep> steps,
                Type commandType
            )
            {
                return _cache.GetOrAdd(commandType, _ => BuildPipeline());

                Func<CommandWithReturnExecutionContext<TConnection, TReturn>, Task<ApiResponse<TReturn>>> BuildPipeline()
                {
                    Func<CommandWithReturnExecutionContext<TConnection, TReturn>, Task<ApiResponse<TReturn>>> pipeline = c
                        => c.Handler.Execute(c.Connection, c.Command);

                    foreach (var step in steps.Reverse())
                    {
                        var pipelineLocal = pipeline;

                        pipeline = c => step.ExecuteWithReturn(pipelineLocal, c);
                    }

                    return pipeline;
                }
            }
        }

        private static class ExecuteCache<TCommand>
            where TCommand : ICommand
        {
            public static Func<CommandExecutionContext<TConnection, TCommand>, Task<ApiResponse<Unit>>> Pipeline { get; private set; }

            public static void EnsureInitialized(ImmutableArray<ICommandPipelineStep> steps)
            {
                if (Pipeline != null)
                {
                    return;
                }

                Func<CommandExecutionContext<TConnection, TCommand>, Task<ApiResponse<Unit>>> pipeline = c
                    => c.Handler.Execute(c.Connection, c.Command);

                foreach (var step in steps.Reverse())
                {
                    var pipelineLocal = pipeline;

                    pipeline = c => step.Execute(pipelineLocal, c);
                }

                Pipeline = pipeline;
            }
        }
    }
}