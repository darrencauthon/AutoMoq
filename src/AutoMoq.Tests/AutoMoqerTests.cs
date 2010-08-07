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
}