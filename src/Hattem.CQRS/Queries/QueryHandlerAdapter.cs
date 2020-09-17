using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Extensions;

namespace Hattem.CQRS.Queries
{
    internal sealed class QueryHandlerAdapter<THandler, TConnection, TQuery, TResult> :
        IQueryHandler<TConnection, IQuery<TResult>, TResult>
        where THandler : class, IQueryHandler<TConnection, TQuery, TResult>
        where TConnection : IHattemConnection
        where TQuery : class, IQuery<TResult>
    {
        private readonly THandler _handler;

        public QueryHandlerAdapter(THandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task<ApiResponse<TResult>> Handle(TConnection connection, IQuery<TResult> query)
        {
            return _handler.Handle(connection, (TQuery) query);
        }
    }
}
