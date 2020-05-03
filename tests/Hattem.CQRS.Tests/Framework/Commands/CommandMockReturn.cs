using System;

namespace Hattem.CQRS.Tests.Framework.Commands
{
    public sealed class CommandMockReturn : IEquatable<CommandMockReturn>
    {
        public Guid Data { get; }

        private CommandMockReturn(Guid data)
        {
            Data = data;
        }

        public static CommandMockReturn New()
        {
            return new CommandMockReturn(Guid.NewGuid());
        }

        public bool Equals(CommandMockReturn other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Data.Equals(other.Data);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is CommandMockReturn other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }
}
