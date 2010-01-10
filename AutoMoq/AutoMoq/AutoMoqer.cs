using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq.Unity;
using Microsoft.Practices.Unity;
using Moq;

namespace AutoMoq
{
    public class AutoMoqer
    {
        private readonly IUnityContainer container;
        private readonly IDictionary<Type, object> registeredMocks;

        public AutoMoqer(IUnityContainer container)
        {
            this.container = container;
            registeredMocks = new Dictionary<Type, object>();

            AddTheAutoMockingContainerExtensionToTheContainer(container);
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public Mock<T> GetMock<T>() where T : class
        {
            var type = GetTheMockType<T>();
            if (GetMockHasNotBeenCalledForThisType(type))
                CreateANewMockAndRegisterIt<T>(type);

            return TheRegisteredMockForThisType<T>(type);
        }

        #region private methods

        private static void AddTheAutoMockingContainerExtensionToTheContainer(IUnityContainer container)
        {
            container.AddNewExtension<AutoMockingContainerExtension>();
            return;
        }

        private Mock<T> TheRegisteredMockForThisType<T>(Type type) where T : class
        {
            return (Mock<T>)registeredMocks.Where(x => x.Key == type).First().Value;
        }

        private void CreateANewMockAndRegisterIt<T>(Type type) where T : class
        {
            var mock = new Mock<T>();
            container.RegisterInstance<T>(mock.Object);
            registeredMocks.Add(type, mock);
        }

        private bool GetMockHasNotBeenCalledForThisType(Type type)
        {
            return registeredMocks.ContainsKey(type) == false;
        }

        private static Type GetTheMockType<T>() where T : class
        {
            return typeof (Mock<T>);
        }

        #endregion
    }
}