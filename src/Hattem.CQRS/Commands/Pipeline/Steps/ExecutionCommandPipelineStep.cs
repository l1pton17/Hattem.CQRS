﻿using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Commands.Pipeline.Steps
{
    internal sealed class ExecutionCommandPipelineStep : ICommandPipelineStep
    {
        public Task<ApiResponse<Unit>> Execute<TConnection, TCommand>(
            ICommandPipelineStep next,
            CommandExecutionContext<TConnection, TCommand> context
        )
            where TConnection : IHattemConnection
            where TCommand : ICommand
        {
            return context.Handler.Execute(context.Connection, context.Command);
        }

        public Task<ApiResponse<TReturn>> ExecuteWithReturn<TConnection, TReturn>(
            ICommandPipelineStep next,
            CommandWithReturnExecutionContext<TConnection, TReturn> context
        )
            where TConnection : IHattemConnection
        {
            return context.Handler.Execute(context.Connection, context.Command);
        }
    }
}