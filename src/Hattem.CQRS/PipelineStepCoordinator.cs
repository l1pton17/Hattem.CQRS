using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Hattem.CQRS
{
    internal interface IPipelineStepCoordinator<TInterfacePipelineStep>
    {
        void Add<TPipelineStep>()
            where TPipelineStep : TInterfacePipelineStep;

        ImmutableArray<TInterfacePipelineStep> Build(
            IEnumerable<TInterfacePipelineStep> steps
        );
    }

    internal sealed class PipelineStepCoordinator<TInterfacePipelineStep> : IPipelineStepCoordinator<TInterfacePipelineStep>
    {
        private readonly List<Type> _stepOrder = new List<Type>();

        public void Add<TPipelineStep>()
            where TPipelineStep : TInterfacePipelineStep
        {
            _stepOrder.Add(typeof(TPipelineStep));
        }

        public ImmutableArray<TInterfacePipelineStep> Build(IEnumerable<TInterfacePipelineStep> steps)
        {
            if (steps == null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            var typeToStep = steps.ToImmutableDictionary(v => v.GetType(), v => v);

            return _stepOrder
                .Select(v => typeToStep[v])
                .ToImmutableArray();
        }
    }
}
