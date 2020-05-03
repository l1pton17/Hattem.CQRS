using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Queries.Pipeline;

namespace Hattem.CQRS.Queries
{
    /// <summary>
    /// Query processor
    /// </summary>
    public interface IQueryProcessor
    {
        /// <summary>
        /// Process a query
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<ApiResponse<TResult>> Process<TResult>(IQuery<TResult> query);
    }

    internal sealed class QueryProcessor<TSession, TConnection> : IQueryProcessor
        where TConnection : IHattemConnection
        where TSession : IHattemSession
    {
        private readonly TConnection _connection;
        private readonly IHandlerProvider<TSession, TConnection> _handlerProvider;
        private readonly IQueryExecutor<TConnection> _executor;

        public QueryProcessor(
            TConnection connection,
            IHandlerProvider<TSession, TConnection> handlerProvider,
            IQueryExecutor<TConnection> executor
        )
        {
            _handlerProvider = handlerProvider ?? throw new ArgumentNullException(nameof(handlerProvider));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public Task<ApiResponse<TResult>> Process<TResult>(IQuery<TResult> query)
        {
            var queryHandler = _handlerProvider.GetQueryHandler<TResult>(query.GetType());

            var context = QueryExecutionContext.Create(
                queryHandler,
                _connection,
                query);

            return _executor.Process(context);
        }
    }
}