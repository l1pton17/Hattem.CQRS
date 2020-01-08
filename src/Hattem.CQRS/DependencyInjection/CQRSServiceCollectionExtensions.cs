using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.DependencyInjection
{
    public static class CQRSServiceCollectionExtensions
    {
        public static CQRSBuilder AddCQRS(this IServiceCollection services)
        {
            var builder = new CQRSBuilder(services);

            services.AddSingleton<ICacheStorage, NoOpCacheStorage>();

            return builder;
        }
    }
}
