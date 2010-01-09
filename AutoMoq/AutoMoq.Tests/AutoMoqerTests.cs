using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AutoMoq.Tests
{
    [TestClass]
    public class AutoMoqerTests
    {
        [TestMethod]
        public void Resolve_InterfacePassed_ReturnsResolutionFromUnityContainer()
        {
            // arrange
            var expectedResult = new Action();

            var containerFake = new Mock<MockUnityContainer>();
            containerFake.Setup(x => x.Resolve<IAction>())
                .Returns(expectedResult);

            var mocker = new AutoMoqer(containerFake.Object);

            // act
            var result = mocker.Resolve<IAction>();

            // assert
            Assert.AreSame(expectedResult, result);
        }

    }

    public interface IAction
    {
    }

    public class Action : IAction
    {
    }
}