using NUnit.Framework;
using Should;

namespace AutoMoq.Tests
{
    [TestFixture]
    public class AutoMoqerTests
    {
        private AutoMoqer mocker;

        [SetUp]
        public void Setup()
        {
            mocker = new AutoMoqer();
        }

        [Test]
        public void Can_resolve_a_concrete_class()
        {
            var concreteClass = mocker.Resolve<ConcreteClass>();
            concreteClass.ShouldNotBeNull();
        }

        [Test]
        public void Can_resolve_a_class_with_dependencies()
        {
            var concreteClass = mocker.Resolve<ClassWithDependencies>();
            concreteClass.ShouldNotBeNull();
        }

        [Test]
        public void Can_resolve_a_nested_dependency_without_injection()
        {
            var concreteClass = mocker.Resolve<ClassWithNestedDependencies>();
            concreteClass.DependencyWithDependencies.Dependency.ShouldNotBeNull();
        }

        [Test]
        public void Can_inject_a_nested_dependency()
        {
            var nestedDependency = mocker.GetMock<IDependency>();
            var concreteClass = mocker.Resolve<ClassWithNestedDependencies>();
            concreteClass.DependencyWithDependencies.Dependency.ShouldBeSameAs(nestedDependency.Object);
        }
    }

    public class ConcreteClass
    {
    }

    public class ClassWithDependencies
    {
        public IDependency Dependency { get; set; }

        public ClassWithDependencies(IDependency dependency)
        {
            Dependency = dependency;
        }
    }

    public interface IDependency
    {
    }

    public class ClassWithNestedDependencies
    {
        public IDependencyWithDependencies DependencyWithDependencies { get; set; }

        public ClassWithNestedDependencies(IDependencyWithDependencies dependencyWithDependencies)
        {
            DependencyWithDependencies = dependencyWithDependencies;
        }
    }

    public interface IDependencyWithDependencies
    {
        IDependency Dependency { get; }
    }
}