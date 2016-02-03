using System;
using AutoMoq.Helpers;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
using Should;

namespace AutoMoq.Tests
{
    [TestFixture]
    public class AutoMoqTestFixtureTests
    {
        [Test]
        public void Subject_should_be_populated_after_construction()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            Assert.That( fixture.Subject, Is.Not.Null);
        }

        [Test]
        public void Dependencies_should_be_accessible()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            IDependency dependency = fixture.Dependency<IDependency>();
            
            Assert.That( dependency, Is.Not.Null );
        }

        [Test]
        public void Mocked_dependencies_should_be_accessible()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            Mock<IDependency> disp = fixture.Mocked<IDependency>();
            
            Assert.That(disp, Is.Not.Null);
        }

        [Test]
        public void ResetSubject_should_give_another_instance_of_type()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            ClassWithDependencies instance1 = fixture.Subject;

            fixture.ResetSubject();

            ClassWithDependencies instance2 = fixture.Subject;

            Assert.AreNotSame(instance1,instance2);
        }

        [Test]
        public void ResetSubject_should_have_different_mock_dependencies()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            var origDependency = fixture.Mocked<IDependency>();

            fixture.ResetSubject();

            Assert.AreNotSame(origDependency, fixture.Mocked<IDependency>());
		}

        [Test]
        public void ResetSubject_should_allow_a_different_config_to_be_passed()
        {
            var fixture = new AutoMoqTestFixture<Apple>();

            var looseConfig = new Config {MockBehavior = MockBehavior.Loose};
            fixture.ResetSubject(looseConfig);

            fixture.Subject.DoSomething(); // expecting no error

            var strictConfig = new Config {MockBehavior = MockBehavior.Strict};
            fixture.ResetSubject(strictConfig);

            var errorHit = false;
            try
            {
                fixture.Subject.DoSomething(); // expecting an error
            }
            catch
            {
                errorHit = true;
            }
            errorHit.ShouldBeTrue();
        }

		[Test]
		public void Mocker_should_be_set()
		{
			var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

			Assert.IsNotNull(fixture.Mocker);
		}

        public class Apple
        {
            private readonly IOrange orange;

            public Apple(IOrange orange)
            {
                this.orange = orange;
            }

            public void DoSomething()
            {
                orange.Something();
            }
        }

        public interface IOrange
        {
            void Something();
        }
    }
}