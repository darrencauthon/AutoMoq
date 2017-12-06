using Automoqer.Unity;
using Moq;
using System;

namespace AutoMoq
{
    public interface IMocking
    {
        bool AMockHasNotBeenRegisteredFor(Type type);
        void RegisterThisMock(object mock, Type type);
        object GetTheRegisteredMockFor(Type type);
        MockCreationResult CreateAMockObjectFor(Type type, MockBehavior mockBehavior);
        MockCreationResult CreateANewMockObjectAndRegisterIt(Type type);
        void SetMock(Type type, object mock);
        void SetInstance<T>(T instance) where T : class;
        Mock<T> GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>() where T : class;
        Mock<T> GetMockByCreatingAMockIfOneHasNotAlreadyBeenCreated<T>(MockBehavior mockBehavior) where T : class;
    }

}