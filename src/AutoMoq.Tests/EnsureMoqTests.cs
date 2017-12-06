using Moq;
using System;
using Xunit;

namespace AutoMoq.Tests.PullRequest
{
    // ReSharper disable InconsistentNaming
    public class EnsureMoqTests
    {
        [Fact]
        public void GetMock_on_interface_returns_mock()
        {
            //Arrange
            var mocker = new AutoMoqer();

            //Act
            var mock = mocker.GetMock<IDependency>();

            //Assert
            Assert.NotNull(mock);
        }

        [Fact]
        public void GetMock_on_concrete_returns_mock()
        {
            //Arrange
            var mocker = new AutoMoqer();

            //Act
            var mock = mocker.GetMock<ConcreteClass>();

            //Assert
            Assert.NotNull(mock);
        }


        [Fact]
        public void Create_doesnt_return_mock()
        {
            //Arrange
            var mocker = new AutoMoqer();

            //Act
            var result = mocker.Create<ConcreteClass>().Do();

            //Assert
            Assert.Equal("hello", result);
        }

        [Fact]
        public void Resolve_is_an_alias_for_create()
        {
            //Arrange
            var mocker = new AutoMoqer();

            //Act
            var result = mocker.Resolve<ConcreteClass>().Do();

            //Assert
            Assert.Equal("hello", result);
        }

        [Fact]
        public void Create_with_dependency_doesnt_return_mock()
        {
            //Arrange
            var mocker = new AutoMoqer();

            //Act
            var result = mocker.Create<VirtualDependency>().VirtualMethod();

            //Assert
            Assert.Equal("hello", result);
        }

        [Fact]
        public void Create_with_mocked_dependency_uses_mock()
        {
            //Arrange
            var mocker = new AutoMoqer();

            mocker.GetMock<VirtualDependency>()
                .Setup(m => m.VirtualMethod())
                .Returns("mocked");

            //Act
            var result = mocker.Create<ClassWithVirtualDependencies>().CallVirtualChild();

            //Assert
            Assert.Equal("mocked", result);
        }


        [Fact]
        public void Create_with_unbound_concerete_dependency_uses_mock()
        {
            //Arrange
            var mocker = new AutoMoqer();

            //Act
            var result = mocker.Create<ClassWithVirtualDependencies>().CallVirtualChild();

            var mockedResult = new Mock<VirtualDependency>().Object.VirtualMethod();

            //Assert
            Assert.Equal((object)mockedResult, result);
        }


        [Fact]
        public void Create_with_constant_concerete_dependency_uses_constant()
        {
            //Arrange
            var mocker = new AutoMoqer();

            var constant = new VirtualDependency() { PropValue = Guid.NewGuid().ToString() };

            mocker.SetInstance(constant);

            //Act
            var result = mocker.Create<ClassWithVirtualDependencies>().GetVirtualProperty();

            //Assert
            Assert.Equal((object)constant.PropValue, result);
        }

        [Fact]
        public void Registering_instance_for_Interface_injects_that_Instance()
        {
            //Arrange
            var mocker = new AutoMoqer();

            var instance = new Dependency();

            mocker.SetInstance<IDependency>(instance);

            //Act
            var result = mocker.Create<ClassWithDependencies>().Dependency;

            //Assert
            Assert.Equal(instance, result);
        }

    }

    #region Test Types

    public class ConcreteClass
    {
        public string Do()
        {
            return "hello";
        }
    }

    public class Dependency : IDependency { }

    public interface IDependency { }

    public class ClassWithDependencies
    {
        public IDependency Dependency { get; set; }

        public ClassWithDependencies(IDependency dependency)
        {
            Dependency = dependency;
        }
    }

    public class ClassWithVirtualDependencies
    {
        private readonly VirtualDependency _virtualDependency;
        public IDependency Dependency { get; set; }

        public ClassWithVirtualDependencies(IDependency dependency, VirtualDependency virtualDependency)
        {
            _virtualDependency = virtualDependency;
            Dependency = dependency;
        }

        public string CallVirtualChild()
        {
            return _virtualDependency.VirtualMethod();
        }

        public string GetVirtualProperty()
        {
            return _virtualDependency.PropValue;
        }
    }

    public class VirtualDependency
    {
        private readonly IDependency _dependency;

        public string PropValue { get; set; }

        public VirtualDependency() { }

        public VirtualDependency(IDependency dependency)
        {
            _dependency = dependency;
        }

        public virtual string VirtualMethod()
        {
            return "hello";
        }
    }
    #endregion

}
