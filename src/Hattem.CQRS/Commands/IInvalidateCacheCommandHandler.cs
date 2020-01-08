﻿using System.Collections.Generic;

namespace Hattem.CQRS.Commands
{
    public interface IInvalidateCacheCommandHandler<in TCommand>
    {
        IEnumerable<(string Key, string Region)> GetCacheKeys(TCommand command);

        IEnumerable<string> GetCacheRegions(TCommand command);
    }
}