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
        public Task<ApiResponse<TResult>> Process<TConnection, TQuery, TResult>(
            Func<QueryExecutionContext<TConnection, TQuery, TResult>, Task<ApiResponse<TResult>>> next,
            QueryExecutionContext<TConnection, TQuery, TResult> context
        )
            where TConnection : IHattemConnection
            where TQuery : IQuery<TResult>
        {
            CapturedContextStorage<TConnection, TQuery, TResult>.CapturedContext.Add(context);

            return next(context);
        }

        public static void AssertContextCaptured<TResult>(Func<QueryExecutionContext<HattemSessionMock, IQuery<TResult>, TResult>, bool> predicate)
        {
            var contains = CapturedContextStorage<HattemSessionMock, IQuery<TResult>, TResult>.CapturedContext.Any(predicate);

            Assert.True(contains);
        }

        public static void AssertContextCaptured<TQuery, TResult>(Func<QueryExecutionContext<HattemSessionMock, TQuery, TResult>, bool> predicate)
            where TQuery : IQuery<TResult>
        {
            var contains = CapturedContextStorage<HattemSessionMock, TQuery, TResult>.CapturedContext.Any(predicate);

            Assert.True(contains);
        }

        private static class CapturedContextStorage<TConnection, TQuery, TResult>
            where TConnection : IHattemConnection
            where TQuery : IQuery<TResult>
        {
            public static ConcurrentBag<QueryExecutionContext<TConnection, TQuery, TResult>> CapturedContext { get; } =
                new ConcurrentBag<QueryExecutionContext<TConnection, TQuery, TResult>>();
        }
    }
}
