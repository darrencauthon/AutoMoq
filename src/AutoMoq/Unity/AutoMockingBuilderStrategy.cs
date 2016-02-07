using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;

namespace AutoMoq.Unity
{
    public class AutoMockingBuilderStrategy : BuilderStrategy
    {
        private readonly IoC ioc;
        private readonly Mocking mocking;

        public AutoMockingBuilderStrategy(Mocking mocking, IoC ioc)
        {
            this.mocking = mocking;
            this.ioc = ioc;
        }

        public MockCreationResult CreateAMockObject(Type type)
        {
            return mocking.CreateAMockObjectFor(type);
        }

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
                    ioc.Resolve(abstractParameter);
                }
                catch
                {
                    var mockObject = mock.ActualObject;
                    ioc.RegisterInstance(mockObject, abstractParameter);
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
            var autoMoqer = ioc.Resolve<AutoMoqer>();
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
            var mocker = ioc.Resolve<AutoMoqer>();
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