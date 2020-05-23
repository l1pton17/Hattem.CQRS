namespace Hattem.CQRS.Notifications
{
    /// <summary>
    /// Options of domain event handler
    /// </summary>
    public sealed class NotificationHandlerOptions
    {
        /// <summary>
        /// Require domain event handler to return successful response
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Order of execution
        /// </summary>
        public int Order { get; private set; }

        private NotificationHandlerOptions()
        {
        }

        public static NotificationHandlerOptions Create(
            bool isRequired = true,
            int order = 0
        )
        {
            return new NotificationHandlerOptions
            {
                IsRequired = isRequired,
                Order = order
            };
        }
    }
}
