using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Moq;

namespace AutoMoq.Unity
{
    internal class AutoMockingBuilderStrategy : BuilderStrategy
    {
        private readonly MockFactory mockFactory;
        private readonly IEnumerable<Type> registeredTypes;
        private readonly IUnityContainer container;

        public AutoMockingBuilderStrategy(IEnumerable<Type> registeredTypes, IUnityContainer container)
        {
            mockFactory = new MockFactory(MockBehavior.Loose);
            this.registeredTypes = registeredTypes;
            this.container = container;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var autoMoqer = container.Resolve<AutoMoqer>();

            var type = GetTheTypeFromTheBuilderContext(context);
            if (AMockObjectShouldBeCreatedForThisType(type))
            {
                var mock = CreateAMockObject(type);
                context.Existing = mock.Object;
                autoMoqer.SetMock(type, mock);
            }
        }

        #region private methods

        private bool AMockObjectShouldBeCreatedForThisType(Type type)
        {
            return TypeIsNotRegistered(type) && type.IsInterface;
        }

        private static Type GetTheTypeFromTheBuilderContext(IBuilderContext context)
        {
            return ((NamedTypeBuildKey) context.OriginalBuildKey).Type;
        }

        private bool TypeIsNotRegistered(Type type)
        {
            return registeredTypes.Any(x => x.Equals(type)) == false;
        }

        private Mock CreateAMockObject(Type type)
        {
            var createMethod = GenerateAnInterfaceMockCreationMethod(type);

            var mock = InvokeTheMockCreationMethod(createMethod);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.CanRead && !propertyInfo.CanWrite && propertyInfo.PropertyType.IsInterface)
                {
                    SetupMockPropertyGetter(type, propertyInfo, mock);
                }
            }
            return mock;
        }

        private void SetupMockPropertyGetter(Type type, PropertyInfo propertyInfo, Mock mock)
        {
            var methodTemplate = GetType().GetMethod("SetupDependency", BindingFlags.Instance | BindingFlags.NonPublic);
            var genericMethod = methodTemplate.MakeGenericMethod(new[] { type, propertyInfo.PropertyType });
            genericMethod.Invoke(this, new object[] { mock, propertyInfo });
        }

        private void SetupDependency<TMock, TDependency>(Mock<TMock> mock, PropertyInfo propertyInfo) where TMock:class
        { 
            // Creates an expression for (x => x.Dependency) where Dependency is a read only 
            // property of type propertyInfo.PropertyType
            var argument = Expression.Parameter(typeof(TMock), "x");
            var getPropertyExpression = Expression.Property(argument, propertyInfo.Name);
            var lambda = Expression.Lambda<Func<TMock, TDependency>>(getPropertyExpression, argument);            
            Expression<Func<TMock, TDependency>> expression = lambda;            

            var dependency = container.Resolve<TDependency>();
            mock.Setup(expression).Returns(dependency);
        }

        private Mock InvokeTheMockCreationMethod(MethodInfo createMethod)
        {
            return (Mock) createMethod.Invoke(mockFactory, new object[] {new List<object>().ToArray()});
        }

        private MethodInfo GenerateAnInterfaceMockCreationMethod(Type type)
        {
            var createMethodWithNoParameters = mockFactory.GetType().GetMethod("Create", EmptyArgumentList());

            return createMethodWithNoParameters.MakeGenericMethod(new[] {type});
        }

        private static Type[] EmptyArgumentList()
        {
            return new[] {typeof (object[])};
        }

        #endregion
    }
}