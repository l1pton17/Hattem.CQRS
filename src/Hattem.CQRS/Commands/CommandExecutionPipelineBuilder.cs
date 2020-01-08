using System;
using Hattem.CQRS.Commands.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.Commands
{
    public sealed class CommandExecutionPipelineBuilder
    {
        private readonly IServiceCollection _services;

        public CommandExecutionPipelineBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public CommandExecutionPipelineBuilder Use<TPipelineStep>()
            where TPipelineStep : class, ICommandPipelineStep
        {
            _services.AddSingleton<ICommandPipelineStep, TPipelineStep>();

            return this;
        }
    }
}
