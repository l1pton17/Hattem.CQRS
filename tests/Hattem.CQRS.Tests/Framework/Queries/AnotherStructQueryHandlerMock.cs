using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Tests.Framework.Queries
{
    public readonly struct AnotherStructQueryMock : IQuery<QueryMockResult>
    {
        public Guid Id { get; }

        private AnotherStructQueryMock(Guid id)
        {
            Id = id;
        }

        public static AnotherStructQueryMock New()
        {
            return new AnotherStructQueryMock(Guid.NewGuid());
        }
    }

    public sealed class AnotherStructQueryHandler : IQueryHandlerMock<AnotherStructQueryMock, QueryMockResult>
    {
        private static readonly ConcurrentDictionary<Guid, ApiResponse<QueryMockResult>> _results =
            new ConcurrentDictionary<Guid, ApiResponse<QueryMockResult>>();

        public Task<ApiResponse<QueryMockResult>> Handle(HattemSessionMock connection, AnotherStructQueryMock query)
        {
            var result = _results[query.Id];

            return Task.FromResult(result);
        }

        public static (AnotherStructQueryMock Query, ApiResponse<QueryMockResult> Response) GetQuery()
        {
            var result = QueryMockResult.New();
            var query = GetQuery(result);

            return (query, ApiResponse.Ok(result));
        }

        public static AnotherStructQueryMock GetQuery(QueryMockResult result)
        {
            return GetQuery(ApiResponse.Ok(result));
        }

        public static AnotherStructQueryMock GetQuery(ApiResponse<QueryMockResult> response)
        {
            var query = AnotherStructQueryMock.New();

            _results.AddOrUpdate(query.Id, response, (_, __) => response);

            return query;
        }
    }
}
