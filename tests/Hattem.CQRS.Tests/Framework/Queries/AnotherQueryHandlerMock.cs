using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Tests.Framework.Queries
{
    public sealed class AnotherQueryMock : IQuery<QueryMockResult>
    {
        public Guid Id { get; }

        private AnotherQueryMock(Guid id)
        {
            Id = id;
        }

        public static AnotherQueryMock New()
        {
            return new AnotherQueryMock(Guid.NewGuid());
        }
    }

    public sealed class AnotherQueryHandler : IQueryHandlerMock<AnotherQueryMock, QueryMockResult>
    {
        private static readonly ConcurrentDictionary<Guid, ApiResponse<QueryMockResult>> _results =
            new ConcurrentDictionary<Guid, ApiResponse<QueryMockResult>>();

        public Task<ApiResponse<QueryMockResult>> Handle(HattemSessionMock connection, AnotherQueryMock query)
        {
            var result = _results[query.Id];

            return Task.FromResult(result);
        }

        public static (AnotherQueryMock Query, ApiResponse<QueryMockResult> Response) GetQuery()
        {
            var result = QueryMockResult.New();
            var query = GetQuery(result);

            return (query, ApiResponse.Ok(result));
        }

        public static AnotherQueryMock GetQuery(QueryMockResult result)
        {
            return GetQuery(ApiResponse.Ok(result));
        }

        public static AnotherQueryMock GetQuery(ApiResponse<QueryMockResult> response)
        {
            var query = AnotherQueryMock.New();

            _results.AddOrUpdate(query.Id, response, (_, __) => response);

            return query;
        }
    }
}