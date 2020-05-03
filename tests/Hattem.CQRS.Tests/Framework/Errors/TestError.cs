using System;
using Hattem.Api;

namespace Hattem.CQRS.Tests.Framework.Errors
{
    public sealed class TestError : Error
    {
        public TestError()
            : base(ErrorCodes.Test, String.Empty)
        {
        }
    }
}