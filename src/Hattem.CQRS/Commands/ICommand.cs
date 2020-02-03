namespace Hattem.CQRS.Commands
{
    /// <summary>
    /// Command
    /// </summary>
    public interface ICommand
    {
    }

    // ReSharper disable once UnusedTypeParameter
    /// <summary>
    /// Command that returns a result
    /// </summary>
    /// <typeparam name="TReturn"></typeparam>
    public interface ICommand<TReturn> : ICommand
    {
    }
}
