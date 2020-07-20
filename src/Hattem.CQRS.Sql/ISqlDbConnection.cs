using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Hattem.CQRS.Sql
{
    public interface ISqlDbConnection
    {
        DbConnection Connection { get; }

        Task OpenAsync();

        ISqlDbTransaction BeginTransaction(IsolationLevel isolationLevel);
    }
}
