using System;
using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Queries.Pipeline
{
    public interface IQueryPipelineStep
    {
        Task<ApiResponse<TResult>> Process<TConnection, TQuery, TResult>(
            Func<QueryExecutionContext<TConnection, TQuery, TResult>, Task<ApiResponse<TResult>>> next,
            QueryExecutionContext<TConnection, TQuery, TResult> context
        )
            where TConnection : IHattemConnection
            where TQuery : IQuery<TResult>;
    }
}
