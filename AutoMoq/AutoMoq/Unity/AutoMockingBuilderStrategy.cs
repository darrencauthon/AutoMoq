using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Moq;

namespace AutoMoq.Unity
{
    public class AutoMockingBuilderStrategy : BuilderStrategy
    {
        private readonly MockFactory mockFactory;
        private readonly IEnumerable<Type> registeredTypes;

        public AutoMockingBuilderStrategy(IEnumerable<Type> registeredTypes)
        {
            mockFactory = new MockFactory(MockBehavior.Loose);
            this.registeredTypes = registeredTypes;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var type = GetTheTypeFromTheBuilderContext(context);
            if (AMockObjectShouldBeCreatedForThisType(type))
                context.Existing = CreateAMockObject(type).Object;
        }

        #region private methods

        private bool AMockObjectShouldBeCreatedForThisType(Type type)
        {
            return TypeIsNotRegistered(type) && type.IsInterface;
        }

        private static Type GetTheTypeFromTheBuilderContext(IBuilderContext context)
        {
            return ((NamedTypeBuildKey)context.OriginalBuildKey).Type;
        }

        private bool TypeIsNotRegistered(Type type)
        {
            return registeredTypes.Any(x => x.Equals(type)) == false;
        }

        private Mock CreateAMockObject(Type type)
        {
            var createMethod = GenerateAnInterfaceMockCreationMethod(type);

            return InvokeTheMockCreationMethod(createMethod);
        }

        private Mock InvokeTheMockCreationMethod(MethodInfo createMethod)
        {
            return (Mock)createMethod.Invoke(mockFactory, new object[] {new List<object>().ToArray()});
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