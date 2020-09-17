using System;
using Hattem.CQRS.Commands.Pipeline;
using Hattem.CQRS.Containers;
using Hattem.CQRS.Extensions;

namespace Hattem.CQRS.Commands
{
    public sealed class CommandExecutionPipelineBuilder
    {
        private readonly IContainerConfigurator _container;
        private readonly IPipelineStepCoordinator<ICommandPipelineStep> _stepCoordinator;

        public CommandExecutionPipelineBuilder(IContainerConfigurator container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _stepCoordinator = new PipelineStepCoordinator<ICommandPipelineStep>();
        }

        public CommandExecutionPipelineBuilder Use<TPipelineStep>()
            where TPipelineStep : class, ICommandPipelineStep
        {
            _stepCoordinator.Add<TPipelineStep>();
            _container.AddSingleton<ICommandPipelineStep, TPipelineStep>();

            return this;
        }

        internal void Build()
        {
            _container.AddSingletonInstance(typeof(IPipelineStepCoordinator<ICommandPipelineStep>), _stepCoordinator);
        }
    }
}
