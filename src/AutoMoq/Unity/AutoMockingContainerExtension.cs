using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace AutoMoq.Unity
{
    internal class AutoMockingContainerExtension : UnityContainerExtension
    {
        private readonly Config config;
        private readonly IoC ioc;
        private readonly Mocking mocking;

        public AutoMockingContainerExtension(Config config, IoC ioc, Mocking mocking)
        {
            this.config = config;
            this.ioc = ioc;
            this.mocking = mocking;
        }

        protected override void Initialize()
        {
            SetBuildingStrategyForBuildingUnregisteredTypes();
        }

        private void SetBuildingStrategyForBuildingUnregisteredTypes()
        {
            var strategy = new MoqBuilderStrategy(ioc, config, mocking);
            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }
    }
}