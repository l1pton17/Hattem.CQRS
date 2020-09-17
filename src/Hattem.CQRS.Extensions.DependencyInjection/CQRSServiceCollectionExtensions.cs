using System;
using Hattem.CQRS.Containers;
using Hattem.CQRS.DependencyInjection;
using Hattem.CQRS.Extensions.DependencyInjection.Containers;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.Extensions.DependencyInjection
{
    public static class CQRSServiceCollectionExtensions
    {
        public static IServiceCollection AddCQRS(
            this IServiceCollection services,
             Action<CQRSBuilder> configure = null)
        {
            services.AddSingleton<IContainerBuilder, ServiceProviderAdapter>();

            var builder = new CQRSBuilder(new ServiceCollectionAdapter(services));

             configure?.Invoke(builder);

             builder.Build();

             return services;
         }
    }
}
