using AutoMoq;
using Unity.Builder;
using Unity.Extension;

namespace Automoqer.Unity
{
    public class AutoMockingContainerExtension : UnityContainerExtension
    {
        private readonly IocContainer ioc;
        private readonly Mocking mocking;

        public AutoMockingContainerExtension(IocContainer ioc, Mocking mocking)
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