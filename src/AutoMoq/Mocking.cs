using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMoq.Unity;
using Moq;

namespace AutoMoq
{
    // ReSharper disable once InconsistentNaming
    public interface Mocking
    {
        //IDictionary<Type, object> RegisteredMocks { get; }
        bool AMockHasNotBeenRegisteredFor(Type type);
        void RegisterThisMock(object mock, Type type);
        object GetTheMockFor(Type type);
        MockCreationResult CreateAMockObjectFor(Type type);
        void CreateANewMockObjectAndRegisterIt<T>(Type type) where T : class;
        void SetMock(Type type, object mock);
        void SetInstance<T>(T instance) where T : class;
        Mock<T> GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>() where T : class;
    }

    public class MockingWithMoq : Mocking
    {
        private readonly IoC ioc;
        private MockRepository mockRepository;

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

        public object GetTheMockFor(Type type)
        {
            return RegisteredMocks.First(x => x.Key == type).Value;
        }

        public MockCreationResult CreateAMockObjectFor(Type type)
        {
            var createMethod = GenerateAnInterfaceMockCreationMethod(type);

            return InvokeTheMockCreationMethod(createMethod);
        }

        public void CreateANewMockObjectAndRegisterIt<T>(Type type) where T : class
        {
            var mock = new Mock<T>();
            ioc.RegisterInstance(mock.Object);
            ioc.Resolve<AutoMoqer>().SetMock(type, mock);
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
            var type = typeof (T);
            if (GetMockHasNotBeenCalledForThisType(type))
                CreateANewMockAndRegisterIt<T>(type);

            return TheRegisteredMockForThisType<T>(type);
        }

        private MethodInfo GenerateAnInterfaceMockCreationMethod(Type type)
        {
            var createMethodWithNoParameters = mockRepository.GetType().GetMethod("Create", EmptyArgumentList());
            return createMethodWithNoParameters.MakeGenericMethod(type);
        }

        private MockCreationResult InvokeTheMockCreationMethod(MethodInfo createMethod)
        {
            var mock = (Mock) createMethod.Invoke(mockRepository, new object[] {new List<object>().ToArray()});

            return new MockCreationResult()
            {
                ActualObject = mock.Object,
                MockObject = mock
            };
        }

        private static Type[] EmptyArgumentList()
        {
            return new[] {typeof (object[])};
        }

        private Mock<T> TheRegisteredMockForThisType<T>(Type type) where T : class
        {
            return (Mock<T>) GetTheMockFor(type);
        }

        private void CreateANewMockAndRegisterIt<T>(Type type) where T : class
        {
            CreateANewMockObjectAndRegisterIt<T>(type);
        }

        private bool GetMockHasNotBeenCalledForThisType(Type type)
        {
            return AMockHasNotBeenRegisteredFor(type);
        }
    }
}