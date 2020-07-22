using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Tests.Framework.Queries
{
    public sealed class QueryMock : IQuery<QueryMockResult>
    {
        public Guid Id { get; }

        private QueryMock(Guid id)
        {
            Id = id;
        }

        public static QueryMock New()
        {
            return new QueryMock(Guid.NewGuid());
        }
    }

    public sealed class QueryHandlerMock : IQueryHandlerMock<QueryMock, QueryMockResult>
    {
        private static readonly ConcurrentDictionary<Guid, ApiResponse<QueryMockResult>> _results =
            new ConcurrentDictionary<Guid, ApiResponse<QueryMockResult>>();

        public Task<ApiResponse<QueryMockResult>> Handle(HattemSessionMock connection, QueryMock query)
        {
            var result = _results[query.Id];

            return Task.FromResult(result);
        }

        public static (QueryMock Query, ApiResponse<QueryMockResult> Response) GetQuery()
        {
            var result = QueryMockResult.New();
            var query = GetQuery(result);

            return (query, ApiResponse.Ok(result));
        }

        public static QueryMock GetQuery(ApiResponse<QueryMockResult> response)
        {
            var query = QueryMock.New();

            _results.AddOrUpdate(query.Id, response, (_, __) => response);

            return query;
        }

        private static QueryMock GetQuery(QueryMockResult result)
        {
            return GetQuery(ApiResponse.Ok(result));
        }
    }
}
