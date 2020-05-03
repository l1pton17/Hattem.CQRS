using System;
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Hattem.CQRS.Tests.Framework
{
    [TraitDiscoverer(CategoryTraitDiscoverer.FullyQualifiedName, CategoryTraitDiscoverer.AssemblyName)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class CategoryTraitAttribute : Attribute, ITraitAttribute
    {
        public string Category { get; }

        public CategoryTraitAttribute(string category)
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));
        }
    }

    public sealed class CategoryTraitDiscoverer : ITraitDiscoverer
    {
        internal const string AssemblyName = nameof(Hattem) + "." + nameof(CQRS) + "." + nameof(Tests);
        internal const string FullyQualifiedName = AssemblyName + "." + nameof(Framework) + "." + nameof(CategoryTraitDiscoverer);

        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var categoryValue = traitAttribute.GetNamedArgument<string>(nameof(CategoryTraitAttribute.Category));

            if (!String.IsNullOrWhiteSpace(categoryValue))
            {
                yield return new KeyValuePair<string, string>("Category", categoryValue);
            }
        }
    }
}
