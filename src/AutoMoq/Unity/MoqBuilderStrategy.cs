using System;
using Microsoft.Practices.Unity;

namespace AutoMoq.Unity
{
    internal class MoqBuilderStrategy : AutoMockingBuilderStrategy
    {
        private readonly Mocking mocking;

        public MoqBuilderStrategy(IoC ioc, Mocking mocking)
            : base(mocking, ioc.Container as IUnityContainer)
        {
            this.mocking = mocking;
        }

        public override MockCreationResult CreateAMockObject(Type type)
        {
            return mocking.CreateAMockObjectFor(type);
        }
    }
}