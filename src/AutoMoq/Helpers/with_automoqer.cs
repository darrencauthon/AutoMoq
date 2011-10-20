using Moq;

namespace AutoMoq.Helpers
{
    public class with_automoqer
    {
        public static AutoMoqer mocker;

        public with_automoqer()
        {
            mocker = new AutoMoqer();
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