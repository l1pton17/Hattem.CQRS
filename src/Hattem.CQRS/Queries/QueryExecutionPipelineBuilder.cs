using System;
using Hattem.CQRS.Queries.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.Queries
{
    public sealed class QueryExecutionPipelineBuilder
    {
        private readonly IServiceCollection _services;

        public QueryExecutionPipelineBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public QueryExecutionPipelineBuilder Use<TPipelineStep>()
            where TPipelineStep : class, IQueryPipelineStep
        {
            _services.AddSingleton<IQueryPipelineStep, TPipelineStep>();

            return this;
        }
    }
}
