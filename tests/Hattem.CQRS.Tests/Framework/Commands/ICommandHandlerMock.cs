using Hattem.CQRS.Commands;

namespace Hattem.CQRS.Tests.Framework.Commands
{
    public interface ICommandHandlerMock<in TCommand> : ICommandHandler<HattemSessionMock, TCommand>
        where TCommand : ICommand
    {
    }
    
    public interface ICommandHandlerMock<in TCommand, TReturn> : ICommandHandler<HattemSessionMock, TCommand, TReturn>
        where TCommand : ICommand<TReturn>
    {
    }
}