using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Extensions;

namespace Hattem.CQRS.Queries
{
    internal sealed class QueryHandlerDiscovery<THandler, TConnection, TQuery, TResult> :
        IQueryHandler<TConnection, IQuery<TResult>, TResult>,
        IHasHandlerName
        where THandler : class, IQueryHandler<TConnection, TQuery, TResult>
        where TConnection : IHattemConnection
        where TQuery : IQuery<TResult>
    {
        private readonly THandler _handler;

        public string Name { get; }

        public QueryHandlerDiscovery(THandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));

            Name = _handler.GetType().GetFriendlyName();
        }

        public Task<ApiResponse<TResult>> Handle(TConnection connection, IQuery<TResult> query)
        {
            return _handler.Handle(connection, (TQuery) query);
        }
    }
}
