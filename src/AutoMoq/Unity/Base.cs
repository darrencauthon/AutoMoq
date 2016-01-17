using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace AutoMoq.Unity
{
    internal abstract class Base : BuilderStrategy
    {
        private readonly IUnityContainer container;
        private readonly IEnumerable<Type> registeredTypes;

        public Base(IEnumerable<Type> registeredTypes, IUnityContainer container)
        {
            this.registeredTypes = registeredTypes;
            this.container = container;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var type = GetTheTypeFromTheBuilderContext(context);
            if (AMockObjectShouldBeCreatedForThisType(type))
            {
                var mock = CreateAMockTrackedByAutoMoq(type);
                context.Existing = mock.Object;
            }

            if (type.GetConstructors().Any() == false) return;

            LoadAbstractDependenciesInTheGreediestConstructor(type);
        }

        internal void LoadAbstractDependenciesInTheGreediestConstructor(Type type)
        {
            var constructor = type.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
            var abstractParameters = constructor.GetParameters()
                .Where(x => x.ParameterType.IsAbstract)
                .Where(x => x.ParameterType.IsInterface == false)
                .Select(x => x.ParameterType);

            foreach (var abstractParameter in abstractParameters)
            {
                var mock = CreateAMockTrackedByAutoMoq(abstractParameter);
                try
                {
                    container.Resolve(abstractParameter);
                }
                catch
                {
                    var thing = mock.Object as object;
                    container.RegisterInstance(abstractParameter, thing);
                }
            }
        }

        internal dynamic CreateAMockTrackedByAutoMoq(Type type)
        {
            var mock = CreateAMockObject(type);
            var autoMoqer = container.Resolve<AutoMoqer>();
            autoMoqer.SetMock(type, mock);
            return mock;
        }

        internal bool AMockObjectShouldBeCreatedForThisType(Type type)
        {
            return ThisTypeIsNotAFunction(type) &&
                   ThisTypeIsNotRegistered(type) &&
                   ThisIsNotTheTypeThatIsBeingResolvedForTesting(type);
        }

        internal bool ThisIsNotTheTypeThatIsBeingResolvedForTesting(Type type)
        {
            var mocker = container.Resolve<AutoMoqer>();
            return (mocker.ResolveType == null || mocker.ResolveType != type);
        }

        internal static bool ThisTypeIsNotAFunction(Type type)
        {
            return type.Name != "Func`1";
        }

        internal static Type GetTheTypeFromTheBuilderContext(IBuilderContext context)
        {
            return (context.OriginalBuildKey).Type;
        }

        internal bool ThisTypeIsNotRegistered(Type type)
        {
            return registeredTypes.Any(x => x.Equals(type)) == false;
        }

        internal dynamic CreateAMockObject(Type type)
        {
            var createMethod = GenerateAnInterfaceMockCreationMethod(type);

            return InvokeTheMockCreationMethod(createMethod);
        }

        internal abstract dynamic InvokeTheMockCreationMethod(MethodInfo createMethod);

        internal abstract MethodInfo GenerateAnInterfaceMockCreationMethod(Type type);

        internal static Type[] EmptyArgumentList()
        {
            return new[] {typeof (object[])};
        }
    }
}