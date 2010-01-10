using AutoMoq.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AutoMoq.Tests
{
    [TestClass]
    public class AutoMoqerTests
    {
        [TestMethod]
        public void Constructor_ClassInstantiated_AddsAutoMockingContainerExtensionToContainer()
        {
            // arrange
            var containerFake = new Mock<MockUnityContainer>();

            // act
            new AutoMoqer(containerFake.Object);

            // assert
            containerFake.Verify(x => x.AddNewExtension<AutoMockingContainerExtension>());
        }

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

        [TestMethod]
        public void GetMock_CalledWithInterface_ReturnsMockObject()
        {
            // arrange
            var mocker = new AutoMoqer(new Mock<MockUnityContainer>().Object);

            // act
            var result = mocker.GetMock<IAction>();

            // assert
            Assert.AreEqual(typeof (Mock<IAction>), result.GetType());
        }

        [TestMethod]
        public void GetMock_CalledWithSameInterfaceTwice_SecondCallReturnsSameRequestAsFirst()
        {
            // arrange
            var mocker = new AutoMoqer(new Mock<MockUnityContainer>().Object);

            // act
            var first = mocker.GetMock<IAction>();
            var second = mocker.GetMock<IAction>();

            // assert
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void GetMock_CalledWithInterface_RegistersMockWithContainer()
        {
            // arrange
            var containerFake = new Mock<MockUnityContainer>();

            var mocker = new AutoMoqer(containerFake.Object);

            // act
            mocker.GetMock<IAction>();

            // assert
            containerFake.Verify(x => x.RegisterInstance(It.IsAny<IAction>()), Times.Once());
        }

        [TestMethod]
        public void GetMock_CalledWithSameInterfaceTwice_RegistersMockWithContainer()
        {
            // arrange
            var containerFake = new Mock<MockUnityContainer>();

            var mocker = new AutoMoqer(containerFake.Object);

            // act
            mocker.GetMock<IAction>();
            mocker.GetMock<IAction>();

            // assert
            containerFake.Verify(x => x.RegisterInstance(It.IsAny<IAction>()), Times.Once());
        }
    }

    public interface IAction
    {
    }

    public class Action : IAction
    {
    }
}