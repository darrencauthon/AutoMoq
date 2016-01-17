using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity;
using Moq;

namespace AutoMoq.Unity
{
    internal class MoqBuilderStrategy : AutoMockingBuilderStrategy
    {
        private readonly MockRepository mockRepository;

        public MoqBuilderStrategy(IEnumerable<Type> registeredTypes, IUnityContainer container)
            : base(registeredTypes, container)
        {
            mockRepository = new MockRepository(MockBehavior.Loose);
        }

        public override MockCreationResult CreateAMockObject(Type type)
        {
            var createMethod = GenerateAnInterfaceMockCreationMethod(type);

            return InvokeTheMockCreationMethod(createMethod);
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
    }
}