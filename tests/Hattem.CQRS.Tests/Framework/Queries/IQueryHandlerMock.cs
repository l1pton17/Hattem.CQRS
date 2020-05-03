using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Tests.Framework.Queries
{
    public interface IQueryHandlerMock<in TQuery, TResult> : IQueryHandler<HattemSessionMock, TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
    }
}