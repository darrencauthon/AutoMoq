using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq.Unity;
using Moq;

namespace AutoMoq
{
    // ReSharper disable once InconsistentNaming
    public interface Mocking
    {
        bool AMockHasNotBeenRegisteredFor(Type type);
        void RegisterThisMock(object mock, Type type);
        object GetTheRegisteredMockFor(Type type);
        MockCreationResult CreateAMockObjectFor(Type type, MockBehavior mockBehavior);
        MockCreationResult CreateANewMockObjectAndRegisterIt(Type type);
        void SetMock(Type type, object mock);
        void SetInstance<T>(T instance) where T : class;
        Mock<T> GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>() where T : class;
        Mock<T> GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>(MockBehavior mockBehavior) where T : class;
    }

    public class MockingWithMoq : Mocking
    {
        private readonly IoC ioc;
        private readonly MockRepository mockRepository;

        public MockingWithMoq(Config config, IoC ioc)
        {
            this.ioc = ioc;
            RegisteredMocks = new Dictionary<Type, object>();
            mockRepository = new MockRepository(config.MockBehavior);
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

        public object GetTheRegisteredMockFor(Type type)
        {
            return RegisteredMocks.First(x => x.Key == type).Value;
        }

        public MockCreationResult CreateAMockObjectFor(Type type, MockBehavior mockBehavior = MockBehavior.Default)
        {
            var types = new[] {typeof (object[])};
            var createMethodWithNoParameters = mockRepository.GetType().GetMethod("Create", types);
            var createMethod = createMethodWithNoParameters.MakeGenericMethod(type);

            var list = new List<object>();
            if (mockBehavior != MockBehavior.Default)
                list.Add(mockBehavior);
            var objects = list.ToArray();
            var mock = (Mock) createMethod.Invoke(mockRepository, new object[] {objects});

            return new MockCreationResult
            {
                ActualObject = mock.Object,
                MockObject = mock
            };
        }

        public MockCreationResult CreateANewMockObjectAndRegisterIt(Type type)
        {
            var result = CreateAMockObjectFor(type);
            var mock = (Mock) result.MockObject;
            ioc.RegisterInstance(mock.Object);
            ioc.Resolve<AutoMoqer>().SetMock(type, mock);
            return result;
        }

        public MockCreationResult CreateANewMockObjectAndRegisterIt<T>(MockBehavior mockBehavior = MockBehavior.Default) where T : class
        {
            var result = CreateAMockObjectFor(typeof (T), mockBehavior);
            var mock = (Mock<T>) result.MockObject;
            ioc.RegisterInstance(mock.Object);
            ioc.Resolve<AutoMoqer>().SetMock(typeof (T), mock);
            return result;
        }

        public void SetMock(Type type, object mock)
        {
            if (AMockHasNotBeenRegisteredFor(type) == false) return;
            RegisterThisMock(mock, type);
        }

        public void SetInstance<T>(T instance) where T : class
        {
            ioc.RegisterInstance(instance);
            SetMock(typeof (T), null);
        }

        public Mock<T> GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>() where T : class
        {
            return GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>(MockBehavior.Default);
        }

        public Mock<T> GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>(MockBehavior mockBehavior) where T : class
        {
            var type = typeof (T);
            if (GetMockHasNotBeenCalledForThisType(type))
                CreateANewMockAndRegisterIt<T>(mockBehavior);

            return TheRegisteredMockForThisType<T>(type);
        }

        private Mock<T> TheRegisteredMockForThisType<T>(Type type) where T : class
        {
            return (Mock<T>) GetTheRegisteredMockFor(type);
        }

        private void CreateANewMockAndRegisterIt<T>(MockBehavior mockBehavior) where T : class
        {
            CreateANewMockObjectAndRegisterIt<T>(mockBehavior);
        }

        private bool GetMockHasNotBeenCalledForThisType(Type type)
        {
            return AMockHasNotBeenRegisteredFor(type);
        }
    }
}