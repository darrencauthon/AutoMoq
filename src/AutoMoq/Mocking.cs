using System;
using System.Collections.Generic;

namespace AutoMoq
{
    // ReSharper disable once InconsistentNaming
    internal interface Mocking
    {
        IDictionary<Type, object> RegisteredMocks { get; }
    }

    public class MockingWithMoq : Mocking
    {
        public MockingWithMoq()
        {
            RegisteredMocks = new Dictionary<Type, object>();
        }

        public IDictionary<Type, object> RegisteredMocks { get; }
    }
}