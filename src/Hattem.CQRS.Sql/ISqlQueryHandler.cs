using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Sql
{
    public interface ISqlQueryHandler<in TQuery, TResult> : IQueryHandler<SqlHattemSession, TQuery, TResult>
        where TQuery : IQuery<TResult>
    {

    }
}
