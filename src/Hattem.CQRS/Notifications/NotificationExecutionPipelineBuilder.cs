using System;
using Hattem.CQRS.Containers;
using Hattem.CQRS.Notifications.Pipeline;
using Hattem.CQRS.Extensions;

namespace Hattem.CQRS.Notifications
{
    public sealed class NotificationExecutionPipelineBuilder
    {
        private readonly IContainerConfigurator _container;
        private readonly IPipelineStepCoordinator<INotificationPipelineStep> _stepCoordinator;

        public NotificationExecutionPipelineBuilder(IContainerConfigurator container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _stepCoordinator = new PipelineStepCoordinator<INotificationPipelineStep>();
        }

        public NotificationExecutionPipelineBuilder Use<TPipelineStep>()
            where TPipelineStep : class, INotificationPipelineStep
        {
            _stepCoordinator.Add<TPipelineStep>();
            _container.AddSingleton<INotificationPipelineStep, TPipelineStep>();

            return this;
        }

        internal void Build()
        {
            _container.AddSingletonInstance(typeof(IPipelineStepCoordinator<INotificationPipelineStep>), _stepCoordinator);
        }
    }
}
