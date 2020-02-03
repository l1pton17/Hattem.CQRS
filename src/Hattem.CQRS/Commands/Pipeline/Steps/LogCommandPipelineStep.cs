using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Extensions;
using Hattem.CQRS.Helpers;
using Microsoft.Extensions.Logging;

namespace Hattem.CQRS.Commands.Pipeline.Steps
{
    internal sealed class LogCommandPipelineStep : ICommandPipelineStep
    {
        private readonly ILogger<LogCommandPipelineStep> _logger;

        public LogCommandPipelineStep(ILogger<LogCommandPipelineStep> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<Unit>> Execute<TConnection, TCommand>(
            Func<CommandExecutionContext<TConnection, TCommand>, Task<ApiResponse<Unit>>> next,
            CommandExecutionContext<TConnection, TCommand> context
        )
            where TConnection : IHattemConnection
            where TCommand : ICommand
        {
            var methodName = context.Handler.GetName();
            var stopwatch = ValueStopwatch.StartNew();

            try
            {
                _logger.LogTrace($"Start executing: {methodName}");

                return await next(context);
            }
            finally
            {
                _logger.LogTrace($"Finished executing {methodName} in {stopwatch.GetElapsedTime().TotalSeconds:0.00}s");
            }
        }

        public async Task<ApiResponse<TReturn>> ExecuteWithReturn<TConnection, TReturn>(
            Func<CommandWithReturnExecutionContext<TConnection, TReturn>, Task<ApiResponse<TReturn>>> next,
            CommandWithReturnExecutionContext<TConnection, TReturn> context
        )
            where TConnection : IHattemConnection
        {
            var methodName = context.Handler.GetName();
            var stopwatch = ValueStopwatch.StartNew();

            try
            {
                _logger.LogTrace($"Start executing: {methodName}");

                return await next(context);
            }
            finally
            {
                _logger.LogTrace($"Finished executing {methodName} in {stopwatch.GetElapsedTime().TotalSeconds:0.00}s");
            }
        }
    }
}