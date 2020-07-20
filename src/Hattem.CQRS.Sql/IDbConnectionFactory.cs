using System.Data.Common;

namespace Hattem.CQRS.Sql
{
    public interface IDbConnectionFactory
    {
        DbConnection Create();
    }
}
