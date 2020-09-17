using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Notifications.Pipeline
{
    public interface INotificationExecutor<TSession>
        where TSession : IHattemSession
    {
        Task<ApiResponse<Unit>> Handle<TNotification>(
            NotificationExecutionContext<TSession, TNotification> context
        )
            where TNotification : INotification;
    }

    internal sealed class NotificationExecutor<TSession> : INotificationExecutor<TSession>
        where TSession : IHattemSession
    {
        private readonly ImmutableArray<INotificationPipelineStep> _steps;

        public NotificationExecutor(
            IEnumerable<INotificationPipelineStep> steps,
            IPipelineStepCoordinator<INotificationPipelineStep> stepCoordinator
        )
        {
            if (steps == null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            if (stepCoordinator == null)
            {
                throw new ArgumentNullException(nameof(stepCoordinator));
            }

            _steps = stepCoordinator.Build(steps);
        }

        public Task<ApiResponse<Unit>> Handle<TNotification>(
            NotificationExecutionContext<TSession, TNotification> context
        )
            where TNotification : INotification
        {
            NotifyCache<TNotification>.EnsureInitialized(_steps);

            return NotifyCache<TNotification>.Pipeline(context);
        }

        private static class NotifyCache<TNotification>
            where TNotification : INotification
        {
            public static NotificationDelegate<TSession, TNotification> Pipeline { get; private set; }

            public static void EnsureInitialized(in ImmutableArray<INotificationPipelineStep> steps)
            {
                if (Pipeline != null)
                {
                    return;
                }

                NotificationDelegate<TSession, TNotification> pipeline = c => c.Handler.Handle(c.Session, c.Notification);

                foreach (var step in steps.Reverse())
                {
                    var pipelineLocal = pipeline;

                    pipeline = c => step.Handle(pipelineLocal, c);
                }

                Pipeline ??= pipeline;
            }
        }
    }
}
