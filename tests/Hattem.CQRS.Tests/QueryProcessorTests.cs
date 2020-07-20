using System.Collections.Generic;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Extensions;
using Hattem.CQRS.Queries;
using Hattem.CQRS.Tests.Framework;
using Hattem.CQRS.Tests.Framework.Errors;
using Hattem.CQRS.Tests.Framework.Queries;
using Hattem.CQRS.Tests.Framework.Queries.Pipeline;
using Xunit;

namespace Hattem.CQRS.Tests
{
    [CategoryTrait("QueryProcessor")]
    public sealed class QueryProcessorTests : TestsBase
    {
        public static IEnumerable<object[]> ProcessTestsData
        {
            get
            {
                var (successQuery, successResponse) = QueryHandlerMock.GetQuery();

                yield return new object[] {successQuery, successResponse};

                var errorResponse = ApiResponse.Error<QueryMockResult>(new TestError());
                var errorQuery = QueryHandlerMock.GetQuery(errorResponse);

                yield return new object[] {errorQuery, errorResponse};

                var (anotherSuccessQuery, anotherSuccessResponse) = AnotherQueryHandler.GetQuery();

                yield return new object[] {anotherSuccessQuery, anotherSuccessResponse};

                var anotherErrorResponse = ApiResponse.Error<QueryMockResult>(new TestError());
                var anotherErrorQuery = AnotherQueryHandler.GetQuery(errorResponse);

                yield return new object[] {anotherErrorQuery, anotherErrorResponse};
            }
        }

        [Fact(DisplayName = "Should execute pipeline and pass connection in context")]
        public async Task Process_ExecutePipelineAndPassConnectionInContext()
        {
            var session = CreateSession();
            var (query, _) = QueryHandlerMock.GetQuery();

            await session.ProcessQuery(query);

            CatchQueryPipelineStep.AssertContextCaptured<QueryMockResult>(
                // ReSharper disable once IsExpressionAlwaysTrue
                context => context.Connection is HattemSessionMock);
        }

        [Fact(DisplayName = "Should execute pipeline and pass handler in context")]
        public async Task Process_ExecutePipelineAndPassHandlerInContext()
        {
            var session = CreateSession();
            var (query, _) = QueryHandlerMock.GetQuery();

            await session.ProcessQuery(query);

            CatchQueryPipelineStep.AssertContextCaptured<QueryMockResult>(
                context => context.Handler is QueryHandlerDiscovery<IQueryHandler<HattemSessionMock, QueryMock, QueryMockResult>, HattemSessionMock, QueryMock, QueryMockResult> handlerDiscovery
                    && handlerDiscovery.Name == typeof(QueryHandlerMock).GetFriendlyName());
        }

        [Fact(DisplayName = "Should execute pipeline and pass query in context")]
        public async Task Process_ExecutePipelineAndPassQueryInContext()
        {
            var session = CreateSession();
            var (query, _) = QueryHandlerMock.GetQuery();

            await session.ProcessQuery(query);

            CatchQueryPipelineStep.AssertContextCaptured<QueryMockResult>(
                context => context.Query is QueryMock actualQuery && actualQuery.Id == query.Id);
        }

        [Theory(DisplayName = "Should execute handler")]
        [MemberData(nameof(ProcessTestsData))]
        public async Task Process_ExecuteHandler(IQuery<QueryMockResult> query, ApiResponse<QueryMockResult> expected)
        {
            var session = CreateSession();

            var actual = await session.ProcessQuery(query);

            Assert.Equal(actual.IsOk, expected.IsOk);
            Assert.Equal(actual.Error?.Code, expected.Error?.Code);
            Assert.Equal(actual.Data, expected.Data);
        }
    }
}
