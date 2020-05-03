using System;
using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Queries.Pipeline
{
    public interface IQueryPipelineStep
    {
        Task<ApiResponse<TResult>> Process<TConnection, TResult>(
            Func<QueryExecutionContext<TConnection, TResult>, Task<ApiResponse<TResult>>> next,
            QueryExecutionContext<TConnection, TResult> context)
            where TConnection : IHattemConnection;
    }
}
