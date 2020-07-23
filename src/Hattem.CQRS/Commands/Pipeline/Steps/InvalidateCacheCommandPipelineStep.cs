using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.Api.Fluent;

namespace Hattem.CQRS.Commands.Pipeline.Steps
{
    internal sealed class InvalidateCacheCommandPipelineStep : ICommandPipelineStep
    {
        private readonly ICacheStorage _cacheStorage;

        public InvalidateCacheCommandPipelineStep(ICacheStorage cacheStorage)
        {
            _cacheStorage = cacheStorage ?? throw new ArgumentNullException(nameof(cacheStorage));
        }

        public Task<ApiResponse<Unit>> Execute<TConnection, TCommand>(
            Func<CommandExecutionContext<TConnection, TCommand>, Task<ApiResponse<Unit>>> next,
            CommandExecutionContext<TConnection, TCommand> context
        )
            where TConnection : IHattemConnection
            where TCommand : ICommand
        {
            return Invalidate(context.Handler, context.Command)
                .Then(_ => next(context))
                .Then(_ => Invalidate(context.Handler, context.Command));
        }

        public Task<ApiResponse<TReturn>> ExecuteWithReturn<TConnection, TCommand, TReturn>(
            Func<CommandWithReturnExecutionContext<TConnection, TCommand, TReturn>, Task<ApiResponse<TReturn>>> next,
            CommandWithReturnExecutionContext<TConnection, TCommand, TReturn> context
        )
            where TConnection : IHattemConnection
            where TCommand : ICommand<TReturn>
        {
            return InvalidateWithReturn(context.Handler, context.Command)
                .Then(_ => next(context))
                .Filter(_ => InvalidateWithReturn(context.Handler, context.Command));
        }

        private Task<ApiResponse<Unit>> InvalidateWithReturn<TConnection, TCommand, TReturn>(
            ICommandHandler<TConnection, TCommand, TReturn> handler,
            TCommand command)
            where TConnection : IHattemConnection
            where TCommand : ICommand<TReturn>
        {
            if (handler is IInvalidateCacheCommandHandler<TCommand> invalidateCacheCommandHandler)
            {
                return _cacheStorage
                    .Invalidate(invalidateCacheCommandHandler.GetCacheKeys(command))
                    .Then(
                        _ => _cacheStorage.InvalidateRegions(
                            invalidateCacheCommandHandler.GetCacheRegions(command)));
            }

            return ApiResponse.OkAsync();
        }

        private Task<ApiResponse<Unit>> Invalidate<TConnection, TCommand>(
            ICommandHandler<TConnection, TCommand> handler,
            TCommand command)
            where TConnection : IHattemConnection
            where TCommand : ICommand
        {
            if (handler is IInvalidateCacheCommandHandler<TCommand> invalidateCacheCommandHandler)
            {
                return _cacheStorage
                    .Invalidate(invalidateCacheCommandHandler.GetCacheKeys(command))
                    .Then(
                        _ => _cacheStorage.InvalidateRegions(
                            invalidateCacheCommandHandler.GetCacheRegions(command)));
            }

            return ApiResponse.OkAsync();
        }
    }
}
