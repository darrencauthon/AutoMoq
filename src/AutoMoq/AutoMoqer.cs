using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Moq;
using Moq.Language.Flow;
using Unity;

[assembly: InternalsVisibleTo("AutoMoq.Tests")]

namespace AutoMoq
{
    public class AutoMoqer
    {
        private IoC ioc;
        private Mocking mocking;
        internal Type ResolveType;

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
            var result = ioc.Resolve<T>();
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
            var result = ioc.Resolve(type);
            ResolveType = null;
            return result;
        }

        /// <summary>
        ///     Gets the mock that was or will be passed to any object created by Create/Resolve.
        /// </summary>
        /// <typeparam name="T">The type of mock to build.</typeparam>
        /// <returns>A mock object of type T.</returns>
        public virtual Mock<T> GetMock<T>(MockBehavior mockBehavior = MockBehavior.Default) where T : class
        {
            ResolveType = null;
            return mocking.GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>(mockBehavior);
        }

        /// <summary>
        ///     Set an instance of type T to be used when resolving an object that needs T.
        /// </summary>
        /// <typeparam name="T">The type of T to register the instance as.</typeparam>
        /// <param name="instance">The instance of type T to use.</param>
        public virtual void SetInstance<T>(T instance) where T : class
        {
            mocking.SetInstance(instance);
        }

        /// <summary>
        ///     Call Setup on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to setup some sort of expression.</typeparam>
        /// <param name="expression">The expression passed to the mock object.</param>
        /// <returns></returns>
        public ISetup<T> Setup<T>(Expression<Action<T>> expression) where T : class
        {
            return GetMock<T>().Setup(expression);
        }

        /// <summary>
        ///     Call Setup on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to setup some sort of expression.</typeparam>
        /// <param name="expression">The expression passed to the mock object.</param>
        /// <returns>The next step in the setup.</returns>
        public ISetup<T, TResult> Setup<T, TResult>(Expression<Func<T, TResult>> expression) where T : class
        {
            return GetMock<T>().Setup(expression);
        }

        /// <summary>
        ///     Call Verify on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to verify some sort of expression.</typeparam>
        /// <param name="expression">The expression to verify.</param>
        public void Verify<T>(Expression<Action<T>> expression) where T : class
        {
            GetMock<T>().Verify(expression);
        }

        /// <summary>
        ///     Call Verify on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to verify some sort of expression.</typeparam>
        /// <param name="expression">The expression to verify.</param>
        /// <param name="failMessage">A message if the expression cannot be verified.</param>
        public void Verify<T>(Expression<Action<T>> expression, string failMessage) where T : class
        {
            GetMock<T>().Verify(expression, failMessage);
        }

        /// <summary>
        ///     Call Verify on the Mock.
        /// </summary>
        /// <typeparam name="T">The type of T to verify some sort of expression.</typeparam>
        /// <param name="expression">The expression to verify.</param>
        /// <param name="times">The number of times this expression should be verified.</param>
        public void Verify<T>(Expression<Action<T>> expression, Times times) where T : class
        {
            GetMock<T>().Verify(expression, times);
        }

        /// <summary>
        ///     Call Verify on the Mock.
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
            ioc = new UnityIoC(config.Container);
            mocking = new MockingWithMoq(config, ioc);

            ioc.Setup(this, config, mocking);
        }

        internal virtual void SetMock(Type type, object mock)
        {
            mocking.SetMock(type, mock);
        }
    }
}