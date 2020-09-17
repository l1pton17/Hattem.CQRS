using System;
using Hattem.CQRS.Containers;

namespace Hattem.CQRS.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static object GetRequiredService(this IContainerBuilder builder, Type serviceType)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var service = builder.GetOrDefault(serviceType);

            if (service == null)
            {
                throw new InvalidOperationException($"No implementation of {serviceType.GetFriendlyName()}");
            }

            return service;
        }
    }
}
