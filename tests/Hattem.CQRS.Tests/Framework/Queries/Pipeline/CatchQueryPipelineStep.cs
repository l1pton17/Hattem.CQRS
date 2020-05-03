using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Queries;
using Hattem.CQRS.Queries.Pipeline;
using Xunit;

namespace Hattem.CQRS.Tests.Framework.Queries.Pipeline
{
    public sealed class CatchQueryPipelineStep : IQueryPipelineStep
    {
        public Task<ApiResponse<TResult>> Process<TConnection, TResult>(
            Func<QueryExecutionContext<TConnection, TResult>, Task<ApiResponse<TResult>>> next,
            QueryExecutionContext<TConnection, TResult> context
        )
            where TConnection : IHattemConnection
        {
            CapturedContextStorage<TConnection, TResult>.CapturedContext.Add(context);

            return next(context);
        }

        public static void AssertContextCaptured<TResult>(Func<QueryExecutionContext<HattemSessionMock, TResult>, bool> predicate)
        {
            var contains = CapturedContextStorage<HattemSessionMock, TResult>.CapturedContext.Any(predicate);

            Assert.True(contains);
        }

        private static class CapturedContextStorage<TConnection, TResult>
            where TConnection : IHattemConnection
        {
            public static ConcurrentBag<QueryExecutionContext<TConnection, TResult>> CapturedContext { get; } =
                new ConcurrentBag<QueryExecutionContext<TConnection, TResult>>();
        }
    }
}