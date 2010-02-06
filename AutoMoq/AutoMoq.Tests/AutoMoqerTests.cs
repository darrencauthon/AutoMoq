using AutoMoq.Unity;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AutoMoq.Tests
{
    [TestClass]
    public class AutoMoqerTests
    {
        [TestMethod]
        public void Constructor_ClassInstantiatedWithNoConstructorArguments_ClassInstantiated()
        {
            // act
            new AutoMoqer();
        }

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
        public void Constructor_ClassInstantiated_RegistersItselfWithContainer()
        {
            // arrange
            var containerFake = new Mock<MockUnityContainer>();

            // act
            var mocker = new AutoMoqer(containerFake.Object);

            // assert
            containerFake.Verify(x => x.RegisterInstance(mocker));
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

        [TestMethod]
        public void SetMock_MockPassed_SetsMockObject()
        {
            // arrange
            var mocker = new AutoMoqer(new Mock<MockUnityContainer>().Object);

            var expectedDependency = new Mock<IDependency>();

            // act
            mocker.SetMock(typeof (IDependency), expectedDependency);

            // assert
            Assert.AreSame(expectedDependency, mocker.GetMock<IDependency>());
        }

        [TestMethod]
        public void SetMock_MockPassedTwice_OnlyAcceptsTheFirst()
        {
            // arrange
            var container = new UnityContainer();
            var mocker = new AutoMoqer(container);

            var expectedDependency = new Mock<IDependency>();

            // act
            mocker.SetMock(typeof (IDependency), expectedDependency);
            mocker.SetMock(typeof (IDependency), new Mock<IDependency>());

            // assert
            Assert.AreSame(expectedDependency, mocker.GetMock<IDependency>());
        }

        [TestMethod]
        public void Setup_CalledWithAction_ReturnsSetupFromMock()
        {
            // arrange
            var mocker = new AutoMoqer(new UnityContainer());

            // act
            var result = mocker.Setup<IDependency>(x => x.Action());

            // assert
            Assert.IsNotNull(result);
        }
    }

    public interface IAction
    {
    }

    public class Action : IAction
    {
        private readonly IDependency dependency;

        public Action()
        {
            dependency.Action();
        }

        public Action(IDependency dependency)
        {
            this.dependency = dependency;
        }
    }

    public interface IDependency
    {
        void Action();

        string Function();
    }

    public class Dependency : IDependency
    {
        public void Action()
        {
        }

        public string Function()
        {
            return string.Empty;
        }
    }
}