using System;

namespace Hattem.CQRS.Containers
{
    public interface IContainerConfigurator
    {
        bool HasComponent(Type serviceType);

        void Add(DependencyLifetime dependencyLifetime, Type serviceType, Type implementationType);

        void AddSingletonInstance(Type serviceType, object instance);
    }
}
