using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AutoMoq.Unity;
using Microsoft.Practices.Unity;
using Moq;
using Moq.Language.Flow;

[assembly: InternalsVisibleTo("AutoMoq.Tests")]

namespace AutoMoq
{
    public class AutoMoqer
    {
        internal readonly MockBehavior DefaultBehavior;
        private IUnityContainer container;
        private IDictionary<Type, object> registeredMocks;
        internal Type ResolveType;

        public AutoMoqer()
        {
            DefaultBehavior = MockBehavior.Default;
            SetupAutoMoqer(new UnityContainer());
        }

        public AutoMoqer(MockBehavior defaultBehavior)
        {
            DefaultBehavior = defaultBehavior;
            SetupAutoMoqer(new UnityContainer());
        }

        internal AutoMoqer(IUnityContainer container)
        {
            SetupAutoMoqer(container);
        }

        /// <summary>
        ///   Creates an instance of type T. Any interface dependencies will be replaced with mocks.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <returns></returns>
        public virtual T Resolve<T>()
        {
            return Create<T>();
        }

        /// <summary>
        ///   Creates an instance of type T. Any interface dependencies will be replaced with mocks.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <returns></returns>
        public virtual T Create<T>()
        {
            ResolveType = typeof(T);
            var result = container.Resolve<T>();
            SetInstance(result);
            ResolveType = null;
            return result;
        }

        /// <summary>
        ///   Gets the mock that was or will be passed to any object created by Create/Resolve.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <returns></returns>
        public virtual Mock<T> GetMock<T>() where T : class
        {
            ResolveType = null;
            var type = GetTheMockType<T>();
            if (GetMockHasNotBeenCalledForThisType(type))
                CreateANewMockAndRegisterIt<T>(type, DefaultBehavior);

            var mock = TheRegisteredMockForThisType<T>(type);
            return mock;
        }

        public virtual Mock<T> GetMock<T>(MockBehavior behavior) where T : class
        {
            ResolveType = null;
            var type = GetTheMockType<T>();
            if (GetMockHasNotBeenCalledForThisType(type))
                CreateANewMockAndRegisterIt<T>(type, behavior);

            var mock = TheRegisteredMockForThisType<T>(type);

            if (behavior != mock.Behavior)
            {
                throw new InvalidOperationException("Unable to change be behaviour of a an existing mock.");
            }

            return mock;
        }

        internal virtual void SetMock(Type type, Mock mock)
        {
            if (registeredMocks.ContainsKey(type) == false)
                registeredMocks.Add(type, mock);
        }

        public virtual void SetInstance<T>(T instance)
        {
            container.RegisterInstance(instance);
            SetMock(GetTheMockType<T>(), null);
        }

        #region private methods

        private void SetupAutoMoqer(IUnityContainer container)
        {
            this.container = container;
            registeredMocks = new Dictionary<Type, object>();
            container.RegisterInstance(this);
            AddTheAutoMockingContainerExtensionToTheContainer(container);
        }

        private static void AddTheAutoMockingContainerExtensionToTheContainer(IUnityContainer container)
        {
            container.AddNewExtension<AutoMockingContainerExtension>();
            return;
        }

        private Mock<T> TheRegisteredMockForThisType<T>(Type type) where T : class
        {
            return (Mock<T>)registeredMocks.Where(x => x.Key == type).First().Value;
        }

        private void CreateANewMockAndRegisterIt<T>(Type type, MockBehavior behavior) where T : class
        {
            var mock = new Mock<T>(behavior);
            container.RegisterInstance(mock.Object);
            SetMock(type, mock);
        }

        private bool GetMockHasNotBeenCalledForThisType(Type type)
        {
            return registeredMocks.ContainsKey(type) == false;
        }

        private static Type GetTheMockType<T>()
        {
            return typeof(T);
        }

        #endregion


        public void VerifyAllMocks()
        {
            foreach (var registeredMock in registeredMocks)
            {
                var mock = registeredMock.Value as Mock;
                if (mock != null)
                    mock.VerifyAll();
            }
        }

        public ISetup<T> Setup<T>(Expression<Action<T>> expression) where T : class
        {
            return GetMock<T>().Setup(expression);
        }

        public ISetup<T, TResult> Setup<T, TResult>(Expression<Func<T, TResult>> expression) where T : class
        {
            return GetMock<T>().Setup(expression);
        }

        public void Verify<T>(Expression<Action<T>> expression) where T : class
        {
            GetMock<T>().Verify(expression);
        }

        public void Verify<T>(Expression<Action<T>> expression, string failMessage) where T : class
        {
            GetMock<T>().Verify(expression, failMessage);
        }

        public void Verify<T>(Expression<Action<T>> expression, Times times) where T : class
        {
            GetMock<T>().Verify(expression, times);
        }

        public void Verify<T>(Expression<Action<T>> expression, Times times, string failMessage) where T : class
        {
            GetMock<T>().Verify(expression, times, failMessage);
        }
    }
}
