using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace AutoMoq.Unity
{
    public abstract class AutoMockingBuilderStrategy : BuilderStrategy
    {
        private readonly Mocking mocking;
        private readonly IUnityContainer container;

        protected AutoMockingBuilderStrategy(Mocking mocking, IUnityContainer container)
        {
            this.mocking = mocking;
            this.container = container;
        }

        public abstract MockCreationResult CreateAMockObject(Type type);

        public override void PreBuildUp(IBuilderContext context)
        {
            var type = GetTheTypeFromTheBuilderContext(context);
            if (AMockObjectShouldBeCreatedForThisType(type))
            {
                var mock = CreateAMockTrackedByAutoMoq(type);
                context.Existing = mock.ActualObject;
            }

            if (type.GetConstructors().Any() == false) return;

            LoadAbstractDependenciesInTheGreediestConstructor(type);
        }

        private void LoadAbstractDependenciesInTheGreediestConstructor(Type type)
        {
            foreach (var abstractParameter in AbstractParameters(type))
            {
                var mock = CreateAMockTrackedByAutoMoq(abstractParameter);
                try
                {
                    container.Resolve(abstractParameter);
                }
                catch
                {
                    var mockObject = mock.ActualObject as object;
                    container.RegisterInstance(abstractParameter, mockObject);
                }
            }
        }

        private static IEnumerable<Type> AbstractParameters(Type type)
        {
            var greediestConstructor = type.GetConstructors()
                .OrderByDescending(x => x.GetParameters().Count())
                .First();
            return greediestConstructor.GetParameters()
                .Where(x => x.ParameterType.IsAbstract)
                .Where(x => x.ParameterType.IsInterface == false)
                .Select(x => x.ParameterType);
        }

        private MockCreationResult CreateAMockTrackedByAutoMoq(Type type)
        {
            var mock = CreateAMockObject(type);
            var autoMoqer = container.Resolve<AutoMoqer>();
            autoMoqer.SetMock(type, mock.MockObject);
            return mock;
        }

        private bool AMockObjectShouldBeCreatedForThisType(Type type)
        {
            return ThisTypeIsNotAFunction(type) &&
                   ThisTypeIsNotRegistered(type) &&
                   ThisIsNotTheTypeThatIsBeingResolvedForTesting(type);
        }

        private bool ThisIsNotTheTypeThatIsBeingResolvedForTesting(Type type)
        {
            var mocker = container.Resolve<AutoMoqer>();
            return (mocker.ResolveType == null || mocker.ResolveType != type);
        }

        private static bool ThisTypeIsNotAFunction(Type type)
        {
            return type.Name != "Func`1";
        }

        private static Type GetTheTypeFromTheBuilderContext(IBuilderContext context)
        {
            return (context.OriginalBuildKey).Type;
        }

        private bool ThisTypeIsNotRegistered(Type type)
        {
            return mocking.AMockHasNotBeenRegisteredFor(type);
        }
    }
}