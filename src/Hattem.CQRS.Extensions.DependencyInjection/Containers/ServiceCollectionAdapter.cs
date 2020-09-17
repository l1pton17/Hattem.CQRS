using System;
using System.Linq;
using Hattem.CQRS.Containers;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.Extensions.DependencyInjection.Containers
{
    internal sealed class ServiceCollectionAdapter : IContainerConfigurator
    {
        private readonly IServiceCollection _services;

        public ServiceCollectionAdapter(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public bool HasComponent(Type serviceType)
        {
            return _services.Any(v => v.ServiceType == serviceType);
        }

        public void Add(DependencyLifetime dependencyLifetime, Type serviceType, Type implementationType)
        {
            switch (dependencyLifetime)
            {
                case DependencyLifetime.Scoped:
                    _services.AddScoped(serviceType, implementationType);

                    break;

                case DependencyLifetime.Transient:
                    _services.AddTransient(serviceType, implementationType);

                    break;

                case DependencyLifetime.Singleton:
                    _services.AddSingleton(serviceType, implementationType);

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dependencyLifetime), dependencyLifetime, $"Unknown value of {nameof(dependencyLifetime)}");
            }
        }

        public void AddSingletonInstance(Type serviceType, object instance)
        {
            _services.AddSingleton(serviceType, instance);
        }
    }
}
