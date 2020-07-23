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
        Task<ApiResponse<TResult>> Process<TQuery, TResult>(
            in QueryExecutionContext<TConnection, TQuery, TResult> context
        )
            where TQuery : IQuery<TResult>;
    }

    internal sealed class QueryExecutor<TConnection> : IQueryExecutor<TConnection>
        where TConnection : IHattemConnection
    {
        private readonly ImmutableArray<IQueryPipelineStep> _steps;

        public QueryExecutor(IEnumerable<IQueryPipelineStep> steps)
        {
            _steps = steps.ToImmutableArray();
        }

        public Task<ApiResponse<TResult>> Process<TQuery, TResult>(in QueryExecutionContext<TConnection, TQuery, TResult> context)
            where TQuery : IQuery<TResult>
        {
            var pipeline = QueryCache<TQuery, TResult>.GetPipeline(_steps, context.Query.GetType());

            return pipeline(context);
        }

        private static class QueryCache<TQuery, TResult>
            where TQuery : IQuery<TResult>
        {
            private static readonly ConcurrentDictionary<Type, Func<QueryExecutionContext<TConnection, TQuery, TResult>, Task<ApiResponse<TResult>>>> _cache =
                new ConcurrentDictionary<Type, Func<QueryExecutionContext<TConnection, TQuery, TResult>, Task<ApiResponse<TResult>>>>();

            public static Func<QueryExecutionContext<TConnection, TQuery, TResult>, Task<ApiResponse<TResult>>> GetPipeline(
                ImmutableArray<IQueryPipelineStep> steps,
                Type queryType
            )
            {
                return _cache.GetOrAdd(queryType, _ => BuildPipeline());

                Func<QueryExecutionContext<TConnection, TQuery, TResult>, Task<ApiResponse<TResult>>> BuildPipeline()
                {
                    Func<QueryExecutionContext<TConnection, TQuery, TResult>, Task<ApiResponse<TResult>>> pipeline = c
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
