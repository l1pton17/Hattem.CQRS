using System;
using System.Threading.Tasks;

namespace Hattem.CQRS.Sql
{
    public interface ISqlDbTransaction
    {
        bool IsCompleted { get; }

        Task CommitAsync();

        Task RollbackAsync();
    }
}
