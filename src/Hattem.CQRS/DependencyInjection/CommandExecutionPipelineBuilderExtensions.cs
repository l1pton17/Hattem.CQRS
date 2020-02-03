using Hattem.CQRS.Commands;
using Hattem.CQRS.Commands.Pipeline.Steps;

namespace Hattem.CQRS.DependencyInjection
{
    public static class CommandExecutionPipelineBuilderExtensions
    {
        public static CommandExecutionPipelineBuilder UseDefault(this CommandExecutionPipelineBuilder builder)
        {
            builder
                .Use<LogCommandPipelineStep>()
                .Use<InvalidateCacheCommandPipelineStep>();

            return builder;
        }
    }
}
