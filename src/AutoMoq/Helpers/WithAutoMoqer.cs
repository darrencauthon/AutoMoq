using AutoMoq;
using Moq;

namespace Automoqer.Helpers
{
    public class WithAutoMoqer
    {
        public static AutoMoqer mocker;

        public WithAutoMoqer()
        {
            mocker = new AutoMoqer();
        }

        public WithAutoMoqer(Config config)
        {
            mocker = new AutoMoqer(config);
        }

        public static Mock<T> GetMock<T>() where T : class
        {
            return mocker.GetMock<T>();
        }

        public static T Create<T>() where T : class
        {
            return mocker.Create<T>();
        }

        public static void SetInstance<T>(T instance) where T : class
        {
            mocker.SetInstance(instance);
        }
    }
}