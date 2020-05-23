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
        private readonly IServiceCollection _services;

        internal CommandExecutionPipelineBuilder CommandExecutionPipelineBuilder { get; private set; }
        
        internal QueryExecutionPipelineBuilder QueryExecutionPipelineBuilder { get; private set; }

        internal NotificationExecutionPipelineBuilder NotificationExecutionPipelineBuilder { get; private set; }

        public CQRSBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
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
            _services.AddSingleton<ICacheStorage, TCacheStorage>();
            _services.AddSingleton<ICommandPipelineStep, InvalidateCacheCommandPipelineStep>();
            _services.AddSingleton<IQueryPipelineStep, CacheQueryPipelineStep>();

            return this;
        }

        public CQRSBuilder UseConnection<TISessionFactory, TSessionFactory, TSession>()
            where TSession : class, IHattemSession, IHattemConnection
            where TISessionFactory : class, IHattemSessionFactory<TSession>
            where TSessionFactory : class, TISessionFactory
        {
            return UseConnection<TISessionFactory, TSessionFactory, TSession, TSession>();
        }

        public CQRSBuilder UseConnection<TISessionFactory, TSessionFactory, TSession, TConnection>()
            where TConnection : class, IHattemConnection
            where TSession : class, IHattemSession
            where TISessionFactory : class, IHattemSessionFactory<TSession>
            where TSessionFactory : class, TISessionFactory
        {
            _services.AddSingleton<TISessionFactory, TSessionFactory>();
            _services.AddSingleton<IHattemConnection, TConnection>();
            _services.AddSingleton<IHattemSessionFactory<TSession>, TSessionFactory>();
            _services.AddSingleton<IHandlerProvider<TSession, TConnection>, HandlerProvider<TSession, TConnection>>();

            _services.AddSingleton<INotificationExecutor<TSession>, NotificationExecutor<TSession>>();
            _services.AddSingleton<INotificationPublisher<TSession>, NotificationPublisher<TSession, TConnection>>();

            _services.AddSingleton<ICommandExecutor<TConnection>, CommandExecutor<TConnection>>();
            _services.AddSingleton<ICommandProcessorFactory<TConnection>, CommandProcessorFactory<TSession, TConnection>>();

            _services.AddSingleton<IQueryExecutor<TConnection>, QueryExecutor<TConnection>>();
            _services.AddSingleton<IQueryProcessorFactory<TConnection>, QueryProcessorFactory<TSession, TConnection>>();

            return this;
        }

        public CQRSBuilder ConfigureNotificationExecution(Action<NotificationExecutionPipelineBuilder> configure)
        {
            NotificationExecutionPipelineBuilder = new NotificationExecutionPipelineBuilder(_services);

            configure(NotificationExecutionPipelineBuilder);

            return this;
        }

        public CQRSBuilder ConfigureCommandExecution(Action<CommandExecutionPipelineBuilder> configure)
        {
            CommandExecutionPipelineBuilder = new CommandExecutionPipelineBuilder(_services);

            configure(CommandExecutionPipelineBuilder);

            return this;
        }

        public CQRSBuilder ConfigureQueryExecution(Action<QueryExecutionPipelineBuilder> configure)
        {
            QueryExecutionPipelineBuilder = new QueryExecutionPipelineBuilder(_services);

            configure(QueryExecutionPipelineBuilder);

            return this;
        }

        public void Done()
        {
            if (CommandExecutionPipelineBuilder == null)
            {
                CommandExecutionPipelineBuilder = new CommandExecutionPipelineBuilder(_services);
            }

            _services.TryAddSingleton<ICacheStorage, NoOpCacheStorage>();
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
                    _services.AddSingleton(interfaceDefinition, type);
                }
            }
        }
    }
}
