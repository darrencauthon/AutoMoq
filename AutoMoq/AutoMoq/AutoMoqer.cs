using System;
using Microsoft.Practices.Unity;

namespace AutoMoq
{
    public class AutoMoqer
    {
        private readonly IUnityContainer container;

        public AutoMoqer(IUnityContainer container)
        {
            this.container = container;
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }
    }
}