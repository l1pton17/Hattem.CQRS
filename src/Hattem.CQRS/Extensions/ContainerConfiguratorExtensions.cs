using System;
using Hattem.CQRS.Containers;

namespace Hattem.CQRS.Extensions
{
    public static class ContainerConfiguratorExtensions
    {
        public static bool HasComponent<TServiceType>(this IContainerConfigurator container)
        {
            return container.HasComponent(typeof(TServiceType));
        }

        public static void AddSingleton(this IContainerConfigurator container, Type interfaceDefinition, Type implementation)
        {
            container.Add(DependencyLifetime.Singleton, interfaceDefinition, implementation);
        }

        public static void AddSingleton<TInterface, TImplementation>(this IContainerConfigurator container)
            where TImplementation : class, TInterface
        {
            container.Add(DependencyLifetime.Singleton, typeof(TInterface), typeof(TImplementation));
        }

        public static void AddTransient(this IContainerConfigurator container, Type interfaceDefinition, Type implementation)
        {
            container.Add(DependencyLifetime.Transient, interfaceDefinition, implementation);
        }

        public static void AddTransient<TInterface, TImplementation>(this IContainerConfigurator container)
            where TImplementation : class, TInterface
        {
            container.AddTransient(typeof(TInterface), typeof(TImplementation));
        }

        public static void AddScope(this IContainerConfigurator container, Type interfaceDefinition, Type implementation)
        {
            container.Add(DependencyLifetime.Scoped, interfaceDefinition, implementation);
        }

        public static void AddScope<TInterface, TImplementation>(this IContainerConfigurator container)
            where TImplementation : class, TInterface
        {
            container.AddScope(typeof(TInterface), typeof(TImplementation));
        }
    }
}
