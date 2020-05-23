using System;
using System.Collections.Generic;
using Hattem.CQRS.Notifications;

namespace Hattem.CQRS.Tests.Framework.Notifications
{
    public sealed class OrderTrackerNotification : INotification
    {
        public List<Type> ExecutedHandlers { get; } = new List<Type>();
    }
}
