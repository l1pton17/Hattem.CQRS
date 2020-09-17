using System;
using Hattem.CQRS.Containers;
using Hattem.CQRS.Extensions;
using Hattem.CQRS.Queries.Pipeline;

namespace Hattem.CQRS.Queries
{
    public sealed class QueryExecutionPipelineBuilder
    {
        private readonly IContainerConfigurator _container;
        private readonly IPipelineStepCoordinator<IQueryPipelineStep> _stepCoordinator;

        public QueryExecutionPipelineBuilder(IContainerConfigurator container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _stepCoordinator = new PipelineStepCoordinator<IQueryPipelineStep>();
        }

        public QueryExecutionPipelineBuilder Use<TPipelineStep>()
            where TPipelineStep : class, IQueryPipelineStep
        {
            _stepCoordinator.Add<TPipelineStep>();
            _container.AddSingleton<IQueryPipelineStep, TPipelineStep>();

            return this;
        }

        internal void Build()
        {
            _container.AddSingletonInstance(typeof(IPipelineStepCoordinator<IQueryPipelineStep>), _stepCoordinator);
        }
    }
}
