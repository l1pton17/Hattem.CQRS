using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Tests.Framework;
using Hattem.CQRS.Tests.Framework.Errors;
using Hattem.CQRS.Tests.Framework.Queries;
using Hattem.CQRS.Tests.Framework.Queries.Pipeline;
using Xunit;

namespace Hattem.CQRS.Tests
{
    [CategoryTrait("StructQueryProcessor")]
    public sealed class StructQueryProcessorTests : TestsBase
    {
        [Fact(DisplayName = "Should execute pipeline and pass connection in context")]
        public async Task Process_ExecutePipelineAndPassConnectionInContext()
        {
            var session = CreateSession();
            var (query, _) = StructQueryHandlerMock.GetQuery();

            await session.ProcessStructQuery(query, Returns<QueryMockResult>.Type);

            CatchQueryPipelineStep.AssertContextCaptured<StructQueryMock, QueryMockResult>(

                // ReSharper disable once IsExpressionAlwaysTrue
                context => context.Connection is HattemSessionMock);
        }

        [Fact(DisplayName = "Should execute pipeline and pass handler in context")]
        public async Task Process_ExecutePipelineAndPassHandlerInContext()
        {
            var session = CreateSession();
            var (query, _) = StructQueryHandlerMock.GetQuery();

            await session.ProcessStructQuery(query, Returns<QueryMockResult>.Type);

            CatchQueryPipelineStep.AssertContextCaptured<StructQueryMock, QueryMockResult>(
                context => context.Handler is StructQueryHandlerMock);
        }

        [Fact(DisplayName = "Should execute pipeline and pass query in context")]
        public async Task Process_ExecutePipelineAndPassQueryInContext()
        {
            var session = CreateSession();
            var (query, _) = StructQueryHandlerMock.GetQuery();

            await session.ProcessStructQuery(query, Returns<QueryMockResult>.Type);

            CatchQueryPipelineStep.AssertContextCaptured<StructQueryMock, QueryMockResult>(
                context => context.Query.Equals(query));
        }

        [Fact(DisplayName = "Should return successful response from handler")]
        public async Task Process_SuccessfulResponse_ReturnsResponseFromHandler()
        {
            var session = CreateSession();
            var (query, expectedResponse) = StructQueryHandlerMock.GetQuery();

            var actualResponse = await session.ProcessStructQuery(query, Returns<QueryMockResult>.Type);

            Assert.Equal(expectedResponse, actualResponse);
        }

        [Fact(DisplayName = "Should return error response from handler")]
        public async Task Process_ErrorResponse_ReturnsResponseFromHandler()
        {
            var session = CreateSession();
            var expectedResponse = ApiResponse.Error<QueryMockResult>(new TestError());
            var query = StructQueryHandlerMock.GetQuery(expectedResponse);

            var actualResponse = await session.ProcessStructQuery(query, Returns<QueryMockResult>.Type);

            Assert.Equal(expectedResponse, actualResponse);
        }

        [Fact(DisplayName = "Should return successful response from multiple handler")]
        public async Task Process_SuccessfulResponseFromMultipleHandlers_ReturnsResponseFromHandler()
        {
            var session = CreateSession();
            var (query, expectedResponse) = StructQueryHandlerMock.GetQuery();

            var actualResponse = await session.ProcessStructQuery(query, Returns<QueryMockResult>.Type);

            Assert.Equal(expectedResponse, actualResponse);

            var (anotherQuery, anotherExpectedResponse) = AnotherStructQueryHandler.GetQuery();

            var anotherActualResponse = await session.ProcessStructQuery(anotherQuery, Returns<QueryMockResult>.Type);

            Assert.Equal(anotherExpectedResponse, anotherActualResponse);
        }
    }
}
