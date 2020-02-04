using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.DependencyInjection
{
    public sealed class CQRSBuilder
    {
        private readonly IServiceCollection _services;

        internal CommandExecutionPipelineBuilder CommandExecutionPipelineBuilder { get; }

        public CQRSBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));

            CommandExecutionPipelineBuilder = new CommandExecutionPipelineBuilder(services);
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
            MapGenericInterfaces(typeof(ICommandHandler<,,>), types);
            MapGenericInterfaces(typeof(INotificationHandler<,>), types);

            return this;
        }

        public CQRSBuilder ConfigureCommandExecution(Action<CommandExecutionPipelineBuilder> configure = null)
        {
            configure ??= b => b.UseDefault();

            configure(CommandExecutionPipelineBuilder);

            return this;
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