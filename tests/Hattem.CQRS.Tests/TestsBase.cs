﻿using Hattem.CQRS.Extensions.DependencyInjection;
using Hattem.CQRS.Tests.Framework;
using Hattem.CQRS.Tests.Framework.Commands.Pipeline;
using Hattem.CQRS.Tests.Framework.Notifications.Pipeline;
using Hattem.CQRS.Tests.Framework.Queries.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.Tests
{
    public abstract class TestsBase
    {
        protected IHattemSessionFactoryMock CreateSessionFactory()
        {
            var services = new ServiceCollection();

            services.AddCQRS(
                builder => builder
                    .AddAssembly(typeof(TestsBase).Assembly)
                    .UseConnection<IHattemSessionFactoryMock, HattemSessionFactoryMock, HattemSessionMock>()
                    .ConfigureQueryExecution(pipeline => pipeline.Use<CatchQueryPipelineStep>())
                    .ConfigureCommandExecution(pipeline => pipeline.Use<CatchCommandPipelineStep>())
                    .ConfigureNotificationExecution(pipeline => pipeline.Use<CatchNotificationPipelineStep>()));

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IHattemSessionFactoryMock>();
        }

        protected HattemSessionMock CreateSession()
        {
            return CreateSessionFactory().Create();
        }
    }
}
