using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Queries.Pipeline
{
    public interface IQueryExecutor<TConnection>
        where TConnection : IHattemConnection
    {
        Task<ApiResponse<TResult>> Process<TResult>(
            QueryExecutionContext<TConnection, TResult> context
        );
    }

    internal sealed class QueryExecutor<TConnection> : IQueryExecutor<TConnection>
        where TConnection : IHattemConnection
    {
        private readonly ImmutableArray<IQueryPipelineStep> _steps;

        public QueryExecutor(IEnumerable<IQueryPipelineStep> steps)
        {
            _steps = steps.ToImmutableArray();
        }

        public Task<ApiResponse<TResult>> Process<TResult>(QueryExecutionContext<TConnection, TResult> context)
        {
            var pipeline = QueryCache<TResult>.GetPipeline(_steps, context.Query.GetType());

            return pipeline(context);
        }

        private static class QueryCache<TResult>
        {
            private static readonly ConcurrentDictionary<Type, Func<QueryExecutionContext<TConnection, TResult>, Task<ApiResponse<TResult>>>> _cache =
                new ConcurrentDictionary<Type, Func<QueryExecutionContext<TConnection, TResult>, Task<ApiResponse<TResult>>>>();

            public static Func<QueryExecutionContext<TConnection, TResult>, Task<ApiResponse<TResult>>> GetPipeline(
                ImmutableArray<IQueryPipelineStep> steps,
                Type commandType
            )
            {
                return _cache.GetOrAdd(commandType, _ => BuildPipeline());

                Func<QueryExecutionContext<TConnection, TResult>, Task<ApiResponse<TResult>>> BuildPipeline()
                {
                    Func<QueryExecutionContext<TConnection, TResult>, Task<ApiResponse<TResult>>> pipeline = c
                        => c.Handler.Handle(c.Connection, c.Query);

                    foreach (var step in steps.Reverse())
                    {
                        var pipelineLocal = pipeline;

                        pipeline = c => step.Process(pipelineLocal, c);
                    }

                    return pipeline;
                }
            }
        }
    }
}