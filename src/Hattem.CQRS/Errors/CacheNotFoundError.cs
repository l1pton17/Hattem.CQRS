using Hattem.Api;

namespace Hattem.CQRS.Errors
{
    /// <summary>
    /// Cache entry not found
    /// </summary>
    [ApiErrorCode(CQRSErrorCodes.CacheNotFound)]
    public sealed class CacheNotFoundError : Error
    {
        public static readonly CacheNotFoundError Default = new CacheNotFoundError();

        private CacheNotFoundError()
            : base(
                CQRSErrorCodes.CacheNotFound,
                "Cache value not found")
        {
        }
    }
}
