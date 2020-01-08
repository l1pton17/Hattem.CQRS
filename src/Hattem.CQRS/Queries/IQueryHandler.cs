using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Queries
{
    /// <summary>
    /// Base interface for query handlers. Do not use explicit.
    /// </summary>
    /// <typeparam name="TConnection"></typeparam>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IQueryHandler<in TConnection, in TQuery, TResult>
        where TQuery : IQuery<TResult>
        where TConnection : IHattemConnection
    {
        Task<ApiResponse<TResult>> Handle(TConnection connection, TQuery query);
    }
}