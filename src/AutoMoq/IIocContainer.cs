using System;

namespace AutoMoq
{
    public interface IIocContainer
    {
        T Resolve<T>();
        object Resolve(Type type);
        void RegisterInstance<T>(T instance);
        void RegisterInstance(object instance, Type type);
        object Container { get; }
        void Setup(AutoMoqer autoMoqer, Config config, Mocking mocking);
    }
}