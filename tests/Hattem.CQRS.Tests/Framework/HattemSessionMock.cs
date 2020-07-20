using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Tests.Framework
{
    public readonly struct HattemSessionMock : IHattemSession, IHattemConnection
    {
        private readonly INotificationPublisher<HattemSessionMock> _notificationPublisher;
        private readonly ICommandProcessor<HattemSessionMock> _commandProcessor;
        private readonly IQueryProcessor<HattemSessionMock> _queryProcessor;

        public string Id { get; }

        public HattemSessionMock(
            INotificationPublisher<HattemSessionMock> notificationPublisher,
            ICommandProcessor<HattemSessionMock> commandProcessor,
            IQueryProcessor<HattemSessionMock> queryProcessor)
        {
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            _commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
            _queryProcessor = queryProcessor ?? throw new ArgumentNullException(nameof(queryProcessor));
            Id = Guid.NewGuid().ToString();
        }

        public Task<ApiResponse<TResult>> ProcessQuery<TResult>(IQuery<TResult> query)
        {
            return _queryProcessor.Process(this, query);
        }

        public Task<ApiResponse<Unit>> ExecuteCommand<TCommand>(TCommand command)
            where TCommand : ICommand
        {
            return _commandProcessor.Execute(this, command);
        }

        public Task<ApiResponse<TReturn>> ExecuteCommandAndReturn<TReturn>(ICommand<TReturn> command)
        {
            return _commandProcessor.ExecuteAndReturn(this, command);
        }

        public Task<ApiResponse<Unit>> PublishNotification<T>(T notification)
            where T : INotification
        {
            return _notificationPublisher.Publish(this, notification);
        }
    }
}
