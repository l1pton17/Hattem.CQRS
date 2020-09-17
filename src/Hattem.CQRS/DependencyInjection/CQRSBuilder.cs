using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Commands.Pipeline;
using Hattem.CQRS.Containers;
using Hattem.CQRS.Extensions;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Notifications.Pipeline;
using Hattem.CQRS.Queries;
using Hattem.CQRS.Queries.Pipeline;

namespace Hattem.CQRS.DependencyInjection
{
    public sealed class CQRSBuilder
    {
        internal CommandExecutionPipelineBuilder CommandExecutionPipelineBuilder { get; }

        internal QueryExecutionPipelineBuilder QueryExecutionPipelineBuilder { get; }

        internal NotificationExecutionPipelineBuilder NotificationExecutionPipelineBuilder { get; }

        public IContainerConfigurator Container { get; }

        public CQRSBuilder(IContainerConfigurator container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            CommandExecutionPipelineBuilder = new CommandExecutionPipelineBuilder(container);
            QueryExecutionPipelineBuilder = new QueryExecutionPipelineBuilder(container);
            NotificationExecutionPipelineBuilder = new NotificationExecutionPipelineBuilder(container);
        }

        /// <summary>
        /// Discover assembly handlers
        /// </summary>
        /// <param name="assembly">Assembly to discovery</param>
        /// <returns></returns>
        public CQRSBuilder AddAssembly(Assembly assembly)
        {
            var types = assembly.ExportedTypes.ToArray();

            MapGenericInterfaces(typeof(IQueryHandler<,,>), types);
            MapGenericInterfaces(typeof(ICommandHandler<,>), types);
            MapGenericInterfaces(typeof(ICommandHandler<,,>), types);
            MapGenericInterfaces(typeof(INotificationHandler<,>), types);

            return this;
        }

        public CQRSBuilder UseConnection<TISessionFactory, TSessionFactory, TSession>()
            where TSession : IHattemSession, IHattemConnection
            where TISessionFactory : class, IHattemSessionFactory<TSession>
            where TSessionFactory : class, TISessionFactory
        {
            return UseConnection<TISessionFactory, TSessionFactory, TSession, TSession>();
        }

        public CQRSBuilder UseConnection<TISessionFactory, TSessionFactory, TSession, TConnection>()
            where TConnection : IHattemConnection
            where TSession : IHattemSession
            where TISessionFactory : class, IHattemSessionFactory<TSession>
            where TSessionFactory : class, TISessionFactory
        {
            Container.AddSingleton<TISessionFactory, TSessionFactory>();
            Container.AddSingleton<IHattemSessionFactory<TSession>, TSessionFactory>();
            Container.AddSingleton<IHandlerProvider<TSession, TConnection>, HandlerProvider<TSession, TConnection>>();

            Container.AddSingleton<INotificationExecutor<TSession>, NotificationExecutor<TSession>>();
            Container.AddSingleton<INotificationPublisher<TSession>, NotificationPublisher<TSession, TConnection>>();

            Container.AddSingleton<ICommandExecutor<TConnection>, CommandExecutor<TConnection>>();
            Container.AddSingleton<ICommandProcessorFactory<TConnection>, CommandProcessorFactory<TSession, TConnection>>();

            Container.AddSingleton<IQueryExecutor<TConnection>, QueryExecutor<TConnection>>();
            Container.AddSingleton<IQueryProcessorFactory<TConnection>, QueryProcessorFactory<TSession, TConnection>>();

            return this;
        }

        public CQRSBuilder ConfigureNotificationExecution(Action<NotificationExecutionPipelineBuilder> configure)
        {
            configure(NotificationExecutionPipelineBuilder);

            return this;
        }

        public CQRSBuilder ConfigureCommandExecution(Action<CommandExecutionPipelineBuilder> configure)
        {
            configure(CommandExecutionPipelineBuilder);

            return this;
        }

        public CQRSBuilder ConfigureQueryExecution(Action<QueryExecutionPipelineBuilder> configure)
        {
            configure(QueryExecutionPipelineBuilder);

            return this;
        }

        public void Build()
        {
            CommandExecutionPipelineBuilder.Build();
            QueryExecutionPipelineBuilder.Build();
            NotificationExecutionPipelineBuilder.Build();

            if (!Container.HasComponent<IHandlerAdapterFactory>())
            {
                Container.AddSingleton<IHandlerAdapterFactory, HandlerAdapterFactory>();
            }
        }

        private void MapGenericInterfaces(
            Type interfaceType,
            IEnumerable<Type> types
        )
        {
            var notAbstractTypes = types
                .Where(v => !v.IsAbstract)
                .Where(v => !v.IsGenericTypeDefinition);

            foreach (var type in notAbstractTypes)
            {
                var interfaceDefinitions = type
                    .GetInterfaces()
                    .Where(
                        v => v.IsGenericType
                            && v.GetGenericTypeDefinition() == interfaceType);

                foreach (var interfaceDefinition in interfaceDefinitions)
                {
                    Container.AddSingleton(interfaceDefinition, type);
                }
            }
        }
    }
}
