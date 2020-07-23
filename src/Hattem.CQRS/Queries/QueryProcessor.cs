using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Queries.Pipeline;

namespace Hattem.CQRS.Queries
{
    /// <summary>
    /// Query processor
    /// </summary>
    public interface IQueryProcessor<in TConnection>
        where TConnection : IHattemConnection
    {
        /// <summary>
        /// Process a query
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection">Connection</param>
        /// <param name="query">Query</param>
        /// <returns></returns>
        Task<ApiResponse<TResult>> Process<TResult>(TConnection connection, IQuery<TResult> query);

        /// <summary>
        /// Process a struct query
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TQuery"></typeparam>
        /// <param name="connection">Connection</param>
        /// <param name="query">Query</param>
        /// <param name="returns"></param>
        /// <returns></returns>
        Task<ApiResponse<TResult>> ProcessStruct<TQuery, TResult>(TConnection connection, in TQuery query, Returns<TResult> returns)
            where TQuery : struct, IQuery<TResult>;
    }

    internal sealed class QueryProcessor<TSession, TConnection> : IQueryProcessor<TConnection>
        where TConnection : IHattemConnection
        where TSession : IHattemSession
    {
        private readonly IHandlerProvider<TSession, TConnection> _handlerProvider;
        private readonly IQueryExecutor<TConnection> _executor;

        public QueryProcessor(
            IHandlerProvider<TSession, TConnection> handlerProvider,
            IQueryExecutor<TConnection> executor
        )
        {
            _handlerProvider = handlerProvider ?? throw new ArgumentNullException(nameof(handlerProvider));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<ApiResponse<TResult>> Process<TResult>(TConnection connection, IQuery<TResult> query)
        {
            var queryHandler = _handlerProvider.GetQueryHandler<TResult>(query.GetType());

            var context = QueryExecutionContext.Create(
                connection,
                queryHandler,
                query);

            return _executor.Process(context);
        }

        public Task<ApiResponse<TResult>> ProcessStruct<TQuery, TResult>(TConnection connection, in TQuery query, Returns<TResult> _)
            where TQuery : struct, IQuery<TResult>
        {
            var queryHandler = _handlerProvider.GetQueryHandler<TQuery, TResult>();

            var context = QueryExecutionContext.Create(
                connection,
                queryHandler,
                query);

            return _executor.Process(context);
        }
    }
}
