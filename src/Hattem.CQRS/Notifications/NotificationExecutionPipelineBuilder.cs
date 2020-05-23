using System;
using Hattem.CQRS.Notifications.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Hattem.CQRS.Notifications
{
    public sealed class NotificationExecutionPipelineBuilder
    {
        private readonly IServiceCollection _services;

        public NotificationExecutionPipelineBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public NotificationExecutionPipelineBuilder Use<TPipelineStep>()
            where TPipelineStep : class, INotificationPipelineStep
        {
            _services.AddSingleton<INotificationPipelineStep, TPipelineStep>();

            return this;
        }
    }
}
