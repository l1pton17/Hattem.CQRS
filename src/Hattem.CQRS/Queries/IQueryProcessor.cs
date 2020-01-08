using System.Threading.Tasks;
using Hattem.Api;

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
}
