namespace Hattem.CQRS
{
    public interface IHattemSessionFactory<out TSession>
        where TSession : IHattemSession
    {
        TSession Create();
    }
}
