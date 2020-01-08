using System;
using Hattem.CQRS.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.DependencyInjection
{
    public sealed class CQRSBuilder
    {
        internal CommandExecutionPipelineBuilder CommandExecutionPipelineBuilder { get; }

        public CQRSBuilder(IServiceCollection services)
        {
            CommandExecutionPipelineBuilder = new CommandExecutionPipelineBuilder(services);
        }

        public CQRSBuilder ConfigureCommandExecution(Action<CommandExecutionPipelineBuilder> configure)
        {
            configure ??= b => b.UseExecution();

            configure(CommandExecutionPipelineBuilder);

            return this;
        }
    }
}
