using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Hattem.CQRS.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, string> _genericTypeHumanNames = new ConcurrentDictionary<Type, string>();

        public static string GetFriendlyName(this Type type)
        {
            string RemoveBacktick(string value)
            {
                var idx = value.IndexOf('`');

                if (idx > 0)
                {
                    return value.Remove(idx);
                }

                return value;
            }

            if (!type.IsGenericType)
            {
                return type.Name;
            }

            return _genericTypeHumanNames
                .GetOrAdd(
                    type,
                    v => String.Concat(
                        RemoveBacktick(v.Name),
                        "<",
                        String.Join(", ", v.GenericTypeArguments.Select(a => GetFriendlyName(a))),
                        ">"));
        }
    }
}