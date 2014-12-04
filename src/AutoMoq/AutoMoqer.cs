using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AutoMoq.Unity;
using Microsoft.Practices.Unity;
using Moq;
using Moq.Language.Flow;
using System.Reflection;

[assembly: InternalsVisibleTo("AutoMoq.Tests")]

namespace AutoMoq
{
    public class AutoMoqer
    {
        private IDictionary<Type, object> registeredMocks;
        internal Type ResolveType;

        public AutoMoqer()
        {
            SetupAutoMoqer();
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
            ResolveType = typeof (T);
			// REPLACE
			//var result = container.Resolve<T>();

			var biggestCtor = GetConstructorWithMostParameters<T>();
			var mockDependencies = GetMockDependencies(biggestCtor);

			Object result = biggestCtor.Invoke(mockDependencies.Select(m => m.Object).ToArray());

            ResolveType = null;
			return (T)result;
        }

        /// <summary>
        ///   Creates an instance of type. Any interface dependencies will be replaced with mocks.
        /// </summary>
        /// <param name = "type">The type to create</typeparam>
        /// <returns></returns>
        public virtual object Create(Type type)
        {
            ResolveType = type;
			// REPLACE
			//var result = container.Resolve(type);
			Object result = null;
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
                CreateANewMockAndRegisterIt<T>(type);

            return TheRegisteredMockForThisType<T>(type);
        }

		private static ConstructorInfo GetConstructorWithMostParameters<T>()
		{
			var constructors = typeof(T).GetConstructors();

			if (!constructors.Any())
			{
				return typeof(T).GetConstructor(Type.EmptyTypes);
			}

			var maxParameterCount = constructors.Max(c => c.GetParameters().Length);
			return constructors.First(c => c.GetParameters().Count() == maxParameterCount);
		}

		private IList<Mock> GetMockDependencies(ConstructorInfo biggestCtor)
		{
			if (biggestCtor == null) return new List<Mock>();

			return biggestCtor.GetParameters()
				.Select(parameter => BuildMockObject(parameter.ParameterType))
				.ToList();
		}

		protected Mock BuildMockObject(Type type)
		{
			if (GetMockHasNotBeenCalledForThisType (type)) {
				//var mock = new Mock<T> ();
				//container.RegisterInstance(mock.Object);
				Type mockType = typeof(Mock<>).MakeGenericType(type);
				ConstructorInfo mockCtor = mockType.GetConstructor(Type.EmptyTypes);
				Mock instance = mockCtor.Invoke(new object[] { }) as Mock;
				SetMock (type, instance);
				return instance;
			}
			return (Mock)registeredMocks[type];
			//return null;
			//return TheRegisteredMockForThisType(type);
			//Type mockType = typeof(Mock<>).MakeGenericType(type);
			//ConstructorInfo mockCtor = mockType.GetConstructor(Type.EmptyTypes);
			//Mock instance = mockCtor.Invoke(new object[] { }) as Mock;
			//return instance;
		}

        internal virtual void SetMock(Type type, Mock mock)
        {
            if (registeredMocks.ContainsKey(type) == false)
                registeredMocks.Add(type, mock);
        }

        public virtual void SetInstance<T>(T instance) where T : class
        {
			// REPLACE
            //container.RegisterInstance(instance);
			SetMock(GetTheMockType<T>(), null);
        }

        #region private methods

        private void SetupAutoMoqer()
        {
			// REPLACE
            //this.container = container;
            registeredMocks = new Dictionary<Type, object>();

            //container.RegisterInstance(this);
        }
			
        private Mock<T> TheRegisteredMockForThisType<T>(Type type) where T : class
        {
            return (Mock<T>) registeredMocks.Where(x => x.Key == type).First().Value;
        }

        private void CreateANewMockAndRegisterIt<T>(Type type) where T : class
        {
            var mock = new Mock<T>();
            //container.RegisterInstance(mock.Object);
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

        #endregion

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
