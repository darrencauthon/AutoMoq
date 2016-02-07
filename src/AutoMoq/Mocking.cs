using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace AutoMoq
{
    // ReSharper disable once InconsistentNaming
    internal interface Mocking
    {
        //IDictionary<Type, object> RegisteredMocks { get; }
        bool AMockHasNotBeenRegisteredFor(Type type);
        void RegisterThisMock(object mock, Type type);
        object GetTheMockFor(Type type);
    }

    public class MockingWithMoq : Mocking
    {
        public MockingWithMoq()
        {
            RegisteredMocks = new Dictionary<Type, object>();
        }

        public IDictionary<Type, object> RegisteredMocks { get; }

        public bool AMockHasNotBeenRegisteredFor(Type type)
        {
            return RegisteredMocks.ContainsKey(type) == false;
        }

        public void RegisterThisMock(object mock, Type type)
        {
            RegisteredMocks.Add(type, mock);
        }

        public object GetTheMockFor(Type type)
        {
            return RegisteredMocks.First(x => x.Key == type).Value;
        }
    }
}