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
        private IUnityContainer container;
        private IDictionary<Type, object> registeredMocks;
        internal Type ResolveType;
        private IoC ioc;

        public AutoMoqer()
        {
            SetupAutoMoqer(new Config());
        }

        public AutoMoqer(IUnityContainer container)
        {
            var config = new Config {Container = container};
            SetupAutoMoqer(config);
        }

        public AutoMoqer(Config config)
        {
            SetupAutoMoqer(config);
        }

        /// <summary>
        ///     Alias of Create.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <returns>An instance of T.</returns>
        public virtual T Resolve<T>()
        {
            return Create<T>();
        }

        /// <summary>
        ///     Creates an instance of type T. Any interface dependencies will be replaced with mocks.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <returns>An instance of T.</returns>
        public virtual T Create<T>()
        {
            ResolveType = typeof (T);
            var result = container.Resolve<T>();
            ResolveType = null;
            return result;
        }

        /// <summary>
        ///     Creates an instance of type. Any interface dependencies will be replaced with mocks.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <returns>An object of the requested type.</returns>
        public virtual object Create(Type type)
        {
            ResolveType = type;
            var result = container.Resolve(type);
            ResolveType = null;
            return result;
        }

        /// <summary>
        ///     Gets the mock that was or will be passed to any object created by Create/Resolve.
        /// </summary>
        /// <typeparam name="T">The type of mock to build.</typeparam>
        /// <returns>A mock object of type T.</returns>
        public virtual Mock<T> GetMock<T>() where T : class
        {
            ResolveType = null;
            var type = GetTheMockType<T>();
            if (GetMockHasNotBeenCalledForThisType(type))
                CreateANewMockAndRegisterIt<T>(type);

            return TheRegisteredMockForThisType<T>(type);
        }

        internal virtual void SetMock(Type type, Object mock)
        {
            if (registeredMocks.ContainsKey(type) == false)
                registeredMocks.Add(type, mock);
        }

        /// <summary>
        /// Set an instance of type T to be used when resolving an object that needs T.
        /// </summary>
        /// <typeparam name="T">The type of T to register the instance as.</typeparam>
        /// <param name="instance">The instance of type T to use.</param>
        public virtual void SetInstance<T>(T instance) where T : class
        {
            container.RegisterInstance(instance);
            SetMock(GetTheMockType<T>(), null);
        }

        /// <summary>
        /// Call Setup on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to setup some sort of expression.</typeparam>
        /// <param name="expression">The expression passed to the mock object.</param>
        /// <returns></returns>
        public ISetup<T> Setup<T>(Expression<Action<T>> expression) where T : class
        {
            return GetMock<T>().Setup(expression);
        }

        /// <summary>
        /// Call Setup on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to setup some sort of expression.</typeparam>
        /// <param name="expression">The expression passed to the mock object.</param>
        /// <returns>The next step in the setup.</returns>
        public ISetup<T, TResult> Setup<T, TResult>(Expression<Func<T, TResult>> expression) where T : class
        {
            return GetMock<T>().Setup(expression);
        }

        /// <summary>
        /// Call Verify on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to verify some sort of expression.</typeparam>
        /// <param name="expression">The expression to verify.</param>
        public void Verify<T>(Expression<Action<T>> expression) where T : class
        {
            GetMock<T>().Verify(expression);
        }

        /// <summary>
        /// Call Verify on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to verify some sort of expression.</typeparam>
        /// <param name="expression">The expression to verify.</param>
        /// <param name="failMessage">A message if the expression cannot be verified.</param>
        public void Verify<T>(Expression<Action<T>> expression, string failMessage) where T : class
        {
            GetMock<T>().Verify(expression, failMessage);
        }

        /// <summary>
        /// Call Verify on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to verify some sort of expression.</typeparam>
        /// <param name="expression">The expression to verify.</param>
        /// <param name="times">The number of times this expression should be verified.</param>
        public void Verify<T>(Expression<Action<T>> expression, Times times) where T : class
        {
            GetMock<T>().Verify(expression, times);
        }

        /// <summary>
        /// Call Verify on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to verify some sort of expression.</typeparam>
        /// <param name="expression">The expression to verify.</param>
        /// <param name="times">The number of times this expression should be verified.</param>
        /// <param name="failMessage">A message if the expression cannot be verified.</param>
        public void Verify<T>(Expression<Action<T>> expression, Times times, string failMessage) where T : class
        {
            GetMock<T>().Verify(expression, times, failMessage);
        }

        private void SetupAutoMoqer(Config config)
        {
            this.ioc = new UnityIoC(config.Container);
            this.container = config.Container;
            registeredMocks = new Dictionary<Type, object>();

            AddTheAutoMockingContainerExtensionToTheContainer(container, config);
            container.RegisterInstance(this);
        }

        private static void AddTheAutoMockingContainerExtensionToTheContainer(IUnityContainer container, Config config)
        {
            container.RegisterInstance(config);
            container.AddNewExtension<AutoMockingContainerExtension>();
        }

        private Mock<T> TheRegisteredMockForThisType<T>(Type type) where T : class
        {
            return (Mock<T>) registeredMocks.First(x => x.Key == type).Value;
        }

        private void CreateANewMockAndRegisterIt<T>(Type type) where T : class
        {
            var mock = new Mock<T>();
            container.RegisterInstance(mock.Object);
            SetMock(type, mock);
        }

        private bool GetMockHasNotBeenCalledForThisType(Type type)
        {
            return registeredMocks.ContainsKey(type) == false;
        }

        private static Type GetTheMockType<T>() where T : class
        {
            return typeof (T);
        }
    }
}