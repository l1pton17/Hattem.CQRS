namespace Hattem.CQRS.Sql
{
    public interface IConnectionDbFactory
    {
        ISqlDbConnection Create();
    }
}
