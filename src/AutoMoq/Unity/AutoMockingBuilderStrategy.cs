using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity;
using Moq;

namespace AutoMoq.Unity
{
    internal class AutoMockingBuilderStrategy : Base
    {
        private readonly MockRepository mockRepository;

        public AutoMockingBuilderStrategy(IEnumerable<Type> registeredTypes, IUnityContainer container)
            : base(registeredTypes, container)
        {
            mockRepository = new MockRepository(MockBehavior.Loose);
        }

        internal override MethodInfo GenerateAnInterfaceMockCreationMethod(Type type)
        {
            var createMethodWithNoParameters = mockRepository.GetType().GetMethod("Create", EmptyArgumentList());
            return createMethodWithNoParameters.MakeGenericMethod(type);
        }

        internal override Mock InvokeTheMockCreationMethod(MethodInfo createMethod)
        {
            return (Mock) createMethod.Invoke(mockRepository, new object[] {new List<object>().ToArray()});
        }
    }
}