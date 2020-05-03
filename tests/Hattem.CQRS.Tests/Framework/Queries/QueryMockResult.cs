using System;

namespace Hattem.CQRS.Tests.Framework.Queries
{
    public sealed class QueryMockResult : IEquatable<QueryMockResult>
    {
        public Guid Data { get; }

        private QueryMockResult(Guid data)
        {
            Data = data;
        }

        public static QueryMockResult New()
        {
            return new QueryMockResult(Guid.NewGuid());
        }

        public bool Equals(QueryMockResult other)
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
            return ReferenceEquals(this, obj) || obj is QueryMockResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }
}
