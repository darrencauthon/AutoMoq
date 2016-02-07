using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace AutoMoq.Unity
{
    internal class AutoMockingContainerExtension : UnityContainerExtension
    {
        private readonly Config config;
        private readonly IoC ioc;
        private readonly IList<Type> registeredTypes = new List<Type>();

        public AutoMockingContainerExtension(Config config, IoC ioc)
        {
            this.config = config;
            this.ioc = ioc;
        }

        protected override void Initialize()
        {
            SetEventsOnContainerToTrackAllRegisteredTypes();
            SetBuildingStrategyForBuildingUnregisteredTypes();
        }

        #region private methods

        private void SetEventsOnContainerToTrackAllRegisteredTypes()
        {
            Context.Registering += ((sender, e) => RegisterType(e.TypeFrom));
            Context.RegisteringInstance += ((sender, e) => RegisterType(e.RegisteredType));
        }

        private void RegisterType(Type typeToRegister)
        {
            registeredTypes.Add(typeToRegister);
        }

        private void SetBuildingStrategyForBuildingUnregisteredTypes()
        {
            var strategy = new MoqBuilderStrategy(registeredTypes, ioc, config);
            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }

        #endregion
    }
}