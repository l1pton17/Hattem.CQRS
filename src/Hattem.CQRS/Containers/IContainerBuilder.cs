using System;
using System.Collections.Generic;

namespace Hattem.CQRS.Containers
{
    public interface IContainerBuilder
    {
        IEnumerable<object> GetAll(Type serviceType);

        object GetOrDefault(Type serviceType);
    }
}
