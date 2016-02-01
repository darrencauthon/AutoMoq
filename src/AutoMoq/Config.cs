using Microsoft.Practices.Unity;
using Moq;

namespace AutoMoq
{
    public class Config
    {
        public MockBehavior MockBehavior { get; set; }
        public IUnityContainer Container { get; set; }

        public Config()
        {
            MockBehavior = MockBehavior.Loose;
            Container = new UnityContainer();
        }
    }
}