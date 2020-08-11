using System.Threading.Tasks;
using Hattem.Api;
using Xunit;

namespace Hattem.CQRS.Sql.Tests.Framework.Extensions
{
    public static class ApiResponseExtensions
    {
        public static T AssertAndGet<T>(this ApiResponse<T> source)
        {
            Assert.False(source.HasErrors, source.Error?.ToString());

            return source.Data;
        }

        public static async Task<T> AssertAndGet<T>(this Task<ApiResponse<T>> source)
        {
            var response = await source;

            return response.AssertAndGet();
        }
    }
}
