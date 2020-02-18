using System;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.DependencyInjection
{
    public static class CQRSServiceCollectionExtensions
    {
        public static IServiceCollection AddCQRS(
            this IServiceCollection services,
            Action<CQRSBuilder> configure = null)
        {
            var builder = new CQRSBuilder(services);

            configure?.Invoke(builder);

            builder.Done();

            return services;
        }
    }
}
