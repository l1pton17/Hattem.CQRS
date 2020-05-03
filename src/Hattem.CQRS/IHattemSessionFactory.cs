namespace Hattem.CQRS
{
    public interface IHattemSessionFactory<out TSession>
        where TSession : class, IHattemSession
    {
        TSession Create();
    }
}
