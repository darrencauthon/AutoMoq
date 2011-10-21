using System;
using Moq;
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
            var concreteClass = mocker.Create<ConcreteClass>();
            concreteClass.ShouldNotBeNull();
        }

        [Test]
        public void Can_resolve_a_class_with_dependencies()
        {
            var concreteClass = mocker.Create<ClassWithDependencies>();
            concreteClass.ShouldNotBeNull();
        }

        [Test]
        public void Can_resolve_a_class_with_func_dependencies()
        {
            var concreteClass = mocker.Create<ClassWithFuncDependencies>();
            concreteClass.ShouldNotBeNull();
        }

        [Test]
        public void Can_test_against_a_func_dependency_as_if_it_were_not()
        {
            var concreteClass = mocker.Create<ClassWithFuncDependencies>();
            concreteClass.CallSomething();
            mocker.GetMock<IDependency>()
                .Verify(x => x.Something(), Times.Once());
        }

        [Test]
        public void Can_test_with_an_abstract_dependency()
        {
            var concreteClass = mocker.Create<ClassWithAbstractDependenciesAndManyConstructors>();
            concreteClass.CallSomething();
            mocker.GetMock<AbstractDependency>()
                .Verify(x => x.Something(), Times.Once());
        }


        [Test]
        public void Can_test_with_a_class_that_has_many_constructors_and_abstract_dependencies()
        {

        }

        [Test]
        public void setting_default_behaviour_on_container_should_apply_to_all_mocks()
        {
            var autoMoqer = new AutoMoqer(MockBehavior.Strict);
            autoMoqer.GetMock<IDependency>().Behavior.ShouldEqual(MockBehavior.Strict);
        }

        [Test]
        public void default_container_should_have_default_as_mock_behavirou()
        {
            var autoMoqer = new AutoMoqer();
            autoMoqer.GetMock<IDependency>().Behavior.ShouldEqual(MockBehavior.Default);
        }

        [Test]
        public void should_not_be_able_to_change_behaviour_of_existing_mock()
        {
            var autoMoqer = new AutoMoqer();
            autoMoqer.GetMock<IDependency>(MockBehavior.Strict).Behavior.ShouldEqual(MockBehavior.Strict);

            Assert.Throws<InvalidOperationException>(() => autoMoqer.GetMock<IDependency>(MockBehavior.Default));
        }

        [Test]
        public void should_be_able_to_get_existing_mock_again()
        {
            var autoMoqer = new AutoMoqer();
            autoMoqer.GetMock<IDependency>(MockBehavior.Strict).Behavior.ShouldEqual(MockBehavior.Strict);
            autoMoqer.GetMock<IDependency>().Behavior.ShouldEqual(MockBehavior.Strict);
        }

        [Test]
        public void default_container_should_allow_override_of_behaviour_on_mock_creation()
        {
            var autoMoqer = new AutoMoqer();
            autoMoqer.GetMock<IDependency>(MockBehavior.Strict).Behavior.ShouldEqual(MockBehavior.Strict);
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

    public class ClassWithFuncDependencies
    {
        private readonly IDependency dependency;

        public ClassWithFuncDependencies(Func<IDependency> dependency)
        {
            this.dependency = dependency();
        }

        public IDependency GetTheDependency { get { return this.dependency; } }

        public void CallSomething()
        {
            dependency.Something();
        }
    }

    public interface IDependency
    {
        void Something();
    }

    public class ClassWithAbstractDependencies
    {
        private readonly AbstractDependency abstractDependency;

        public ClassWithAbstractDependencies(AbstractDependency abstractDependency)
        {
            this.abstractDependency = abstractDependency;
        }

        public void CallSomething()
        {
            abstractDependency.Something();
        }
    }

    public class ClassWithAbstractDependenciesAndManyConstructors
    {
        private readonly AbstractDependency abstractDependency;

        public ClassWithAbstractDependenciesAndManyConstructors()
        {
        }

        public ClassWithAbstractDependenciesAndManyConstructors(AbstractDependency abstractDependency)
        {
            this.abstractDependency = abstractDependency;
        }

        public ClassWithAbstractDependenciesAndManyConstructors(IDependency dependency, AbstractDependency abstractDependency)
        {
            this.abstractDependency = abstractDependency;
        }

        public void CallSomething()
        {
            abstractDependency.Something();
        }
    }

    public abstract class AbstractDependency
    {
        public virtual void Something()
        {

        }
    }

}