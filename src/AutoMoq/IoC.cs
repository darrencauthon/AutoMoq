using System;
using Microsoft.Practices.Unity;

namespace AutoMoq
{
    internal interface IoC
    {
        T Resolve<T>();
        object Resolve(Type type);
        void RegisterInstance<T>(T instance);
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
    }
}