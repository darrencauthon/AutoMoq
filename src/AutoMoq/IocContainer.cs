using Automoqer.Unity;
using System;
using Unity;

namespace AutoMoq
{
    public class IocContainer : IIocContainer
    {
        private readonly IUnityContainer container;

        public IocContainer()
        {
            this.container = new UnityContainer();
        }

        public IocContainer(IUnityContainer container)
        {
            this.container = container;
        }

        public T Resolve<T>()
        {
            return this.container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return container.Resolve(type);
        }

        public void RegisterInstance<T>(T instance)
        {
            container.RegisterInstance<T>(instance);
        }

        public void RegisterInstance(object instance, Type type)
        {
            container.RegisterInstance(type, instance);
        }

        public object Container { get { return container; } }

        public void Setup(AutoMoqer autoMoqer, Config config, Mocking mocking)
        {
            AddTheAutoMockingContainerExtensionToTheContainer(autoMoqer, config, mocking);
            RegisterInstance(this);
        }

        private void AddTheAutoMockingContainerExtensionToTheContainer(AutoMoqer automoqer, Config config, Mocking mocking)
        {
            var container = config.Container;
            container.RegisterInstance(config);
            container.RegisterInstance(automoqer);
            container.RegisterInstance<IocContainer>(this);
            container.RegisterInstance<Mocking>(mocking);
            container.AddNewExtension<AutoMockingContainerExtension>();
        }
    }
}