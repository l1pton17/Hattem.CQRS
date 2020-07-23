using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Tests.Framework.Queries
{
    public readonly struct StructQueryMock : IQuery<QueryMockResult>
    {
        public Guid Id { get; }

        private StructQueryMock(Guid id)
        {
            Id = id;
        }

        public static StructQueryMock New()
        {
            return new StructQueryMock(Guid.NewGuid());
        }
    }

    public sealed class StructQueryHandlerMock : IQueryHandlerMock<StructQueryMock, QueryMockResult>
    {
        private static readonly ConcurrentDictionary<Guid, ApiResponse<QueryMockResult>> _results =
            new ConcurrentDictionary<Guid, ApiResponse<QueryMockResult>>();

        public Task<ApiResponse<QueryMockResult>> Handle(HattemSessionMock connection, StructQueryMock query)
        {
            var result = _results[query.Id];

            return Task.FromResult(result);
        }

        public static (StructQueryMock Query, ApiResponse<QueryMockResult> Response) GetQuery()
        {
            var result = QueryMockResult.New();
            var query = GetQuery(result);

            return (query, ApiResponse.Ok(result));
        }

        public static StructQueryMock GetQuery(ApiResponse<QueryMockResult> response)
        {
            var query = StructQueryMock.New();

            _results.AddOrUpdate(query.Id, response, (_, __) => response);

            return query;
        }

        private static StructQueryMock GetQuery(QueryMockResult result)
        {
            return GetQuery(ApiResponse.Ok(result));
        }
    }
}
