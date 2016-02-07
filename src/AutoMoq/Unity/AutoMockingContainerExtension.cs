using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace AutoMoq.Unity
{
    internal class AutoMockingContainerExtension : UnityContainerExtension
    {
        private readonly IoC ioc;
        private readonly Mocking mocking;

        public AutoMockingContainerExtension(IoC ioc, Mocking mocking)
        {
            this.ioc = ioc;
            this.mocking = mocking;
        }

        protected override void Initialize()
        {
            SetBuildingStrategyForBuildingUnregisteredTypes();
        }

        private void SetBuildingStrategyForBuildingUnregisteredTypes()
        {
            var strategy = new AutoMockingBuilderStrategy(mocking, ioc);
            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }
    }
}