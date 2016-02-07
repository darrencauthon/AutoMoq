using System;
using AutoMoq.Unity;
using Microsoft.Practices.Unity;

namespace AutoMoq
{
    internal interface IoC
    {
        T Resolve<T>();
        object Resolve(Type type);
        void RegisterInstance<T>(T instance);
        void RegisterInstance(object instance);
        object Container { get; }
        void Setup(AutoMoqer autoMoqer, Config config, Mocking mocking);
    }

    public class UnityIoC : IoC
    {
        private readonly IUnityContainer container;

        public UnityIoC()
        {
            this.container = new UnityContainer();
        }

        public UnityIoC(IUnityContainer container)
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

        public void RegisterInstance(object instance)
        {
            container.RegisterInstance(instance);
        }

        public object Container { get { return container;  } }

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
            container.RegisterInstance<IoC>(this);
            container.RegisterInstance<Mocking>(mocking);
            container.AddNewExtension<AutoMockingContainerExtension>();
        }
    }
}