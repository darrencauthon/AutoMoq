using Automoqer.Unity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoMoq
{
    public class Mocking : IMocking
    {
        private readonly IocContainer ioc;
        private readonly MockRepository mockRepository;

        public Mocking(Config config, IocContainer ioc)
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
            var createMethod = mockRepository.GetType()
                .GetMethod("Create", new[] { typeof(object[]) }).MakeGenericMethod(type);

            var parameters = new List<object>();
            if (mockBehavior != MockBehavior.Default) parameters.Add(mockBehavior);
            var mock = (Mock)createMethod.Invoke(mockRepository, new object[] { parameters.ToArray() });

            return new MockCreationResult
            {
                ActualObject = mock.Object,
                MockObject = mock
            };
        }

        public MockCreationResult CreateANewMockObjectAndRegisterIt(Type type)
        {
            var result = CreateAMockObjectFor(type);

            var mock = (Mock)result.MockObject;

            RegisterThisObjectInTheIoCContainer(type, mock);
            RegisterThisMockWithAutoMoq(type, mock);

            return result;
        }

        private void RegisterThisMockWithAutoMoq(Type type, Mock mock)
        {
            ioc.Resolve<AutoMoqer>().SetMock(type, mock);
        }

        private void RegisterThisObjectInTheIoCContainer(Type type, Mock mock)
        {
            // this is meant to replicate this generic method call
            // container.RegisterInstance<T>(mock.Object)
            ioc.GetType()
                .GetMethods()
                .First(x => x.Name == "RegisterInstance" && x.IsGenericMethod)
                .MakeGenericMethod(type)
                .Invoke(ioc, new[] { mock.Object });
        }

        public MockCreationResult CreateANewMockObjectAndRegisterIt<T>(MockBehavior mockBehavior = MockBehavior.Default) where T : class
        {
            var result = CreateAMockObjectFor(typeof(T), mockBehavior);
            var mock = (Mock<T>)result.MockObject;
            ioc.RegisterInstance(mock.Object);
            ioc.Resolve<AutoMoqer>().SetMock(typeof(T), mock);
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
            SetMock(typeof(T), null);
        }

        public Mock<T> GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>() where T : class
        {
            return GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>(MockBehavior.Default);
        }

        public Mock<T> GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>(MockBehavior mockBehavior) where T : class
        {
            var type = typeof(T);
            if (GetMockHasNotBeenCalledForThisType(type))
                CreateANewMockAndRegisterIt<T>(mockBehavior);

            return TheRegisteredMockForThisType<T>(type);
        }

        private Mock<T> TheRegisteredMockForThisType<T>(Type type) where T : class
        {
            return (Mock<T>)GetTheRegisteredMockFor(type);
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