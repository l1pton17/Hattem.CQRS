using Hattem.CQRS.Commands;
using Hattem.CQRS.Commands.Pipeline.Steps;

namespace Hattem.CQRS.DependencyInjection
{
    public static class CommandExecutionPipelineBuilderExtensions
    {
        public static CommandExecutionPipelineBuilder UseExecution(this CommandExecutionPipelineBuilder builder)
        {
            builder.Use<ExecutionCommandPipelineStep>();

            return builder;
        }
    }
}
