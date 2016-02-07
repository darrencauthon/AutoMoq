using Microsoft.Practices.Unity;

namespace AutoMoq
{
    internal interface IoC
    {
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
    }
}