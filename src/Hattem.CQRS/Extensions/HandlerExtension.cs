namespace Hattem.CQRS.Extensions
{
    internal static class HandlerExtension
    {
        public static string GetName(this object handler)
        {
            if (handler is IHasHandlerName hasHandlerName)
            {
                return hasHandlerName.Name;
            }

            return handler.GetType().GetFriendlyName();
        }
    }
}
