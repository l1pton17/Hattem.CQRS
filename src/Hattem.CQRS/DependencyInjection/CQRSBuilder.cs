using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Commands.Pipeline;
using Hattem.CQRS.Commands.Pipeline.Steps;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Notifications.Pipeline;
using Hattem.CQRS.Queries;
using Hattem.CQRS.Queries.Pipeline;
using Hattem.CQRS.Queries.Pipeline.Steps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hattem.CQRS.DependencyInjection
{
    public sealed class CQRSBuilder
    {
        internal CommandExecutionPipelineBuilder CommandExecutionPipelineBuilder { get; private set; }
        
        internal QueryExecutionPipelineBuilder QueryExecutionPipelineBuilder { get; private set; }

        internal NotificationExecutionPipelineBuilder NotificationExecutionPipelineBuilder { get; private set; }

        public IServiceCollection Services { get; }

        public CQRSBuilder(IServiceCollection services)
        { 
            Services = services ?? throw new ArgumentNullException(nameof(services));
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

        public CQRSBuilder UseCacheStorage<TCacheStorage>()
            where TCacheStorage : class, ICacheStorage
        {
            Services.AddSingleton<ICacheStorage, TCacheStorage>();
            Services.AddSingleton<ICommandPipelineStep, InvalidateCacheCommandPipelineStep>();
            Services.AddSingleton<IQueryPipelineStep, CacheQueryPipelineStep>();

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
            Services.AddSingleton<TISessionFactory, TSessionFactory>();
            Services.AddSingleton<IHattemSessionFactory<TSession>, TSessionFactory>();
            Services.AddSingleton<IHandlerProvider<TSession, TConnection>, HandlerProvider<TSession, TConnection>>();

            Services.AddSingleton<INotificationExecutor<TSession>, NotificationExecutor<TSession>>();
            Services.AddSingleton<INotificationPublisher<TSession>, NotificationPublisher<TSession, TConnection>>();

            Services.AddSingleton<ICommandExecutor<TConnection>, CommandExecutor<TConnection>>();
            Services.AddSingleton<ICommandProcessorFactory<TConnection>, CommandProcessorFactory<TSession, TConnection>>();

            Services.AddSingleton<IQueryExecutor<TConnection>, QueryExecutor<TConnection>>();
            Services.AddSingleton<IQueryProcessorFactory<TConnection>, QueryProcessorFactory<TSession, TConnection>>();

            return this;
        }

        public CQRSBuilder ConfigureNotificationExecution(Action<NotificationExecutionPipelineBuilder> configure)
        {
            NotificationExecutionPipelineBuilder = new NotificationExecutionPipelineBuilder(Services);

            configure(NotificationExecutionPipelineBuilder);

            return this;
        }

        public CQRSBuilder ConfigureCommandExecution(Action<CommandExecutionPipelineBuilder> configure)
        {
            CommandExecutionPipelineBuilder = new CommandExecutionPipelineBuilder(Services);

            configure(CommandExecutionPipelineBuilder);

            return this;
        }

        public CQRSBuilder ConfigureQueryExecution(Action<QueryExecutionPipelineBuilder> configure)
        {
            QueryExecutionPipelineBuilder = new QueryExecutionPipelineBuilder(Services);

            configure(QueryExecutionPipelineBuilder);

            return this;
        }

        public void Done()
        {
            Services.TryAddSingleton<ICacheStorage, NoOpCacheStorage>();
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
                    Services.AddSingleton(interfaceDefinition, type);
                }
            }
        }
    }
}
