using System;
using System.Collections.Generic;
using Hattem.CQRS.Containers;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.Extensions.DependencyInjection.Containers
{
    internal sealed class ServiceProviderAdapter : IContainerBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderAdapter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IEnumerable<object> GetAll(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }

        public object GetOrDefault(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}
