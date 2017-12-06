using Automoqer.Helpers;
using Moq;
using Xunit;

namespace AutoMoq.Tests
{
    public class AutoMoqTestFixtureTests
    {
        [Fact]
        public void Subject_should_be_populated_after_construction()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            Assert.NotNull(fixture.Subject);
        }

        [Fact]
        public void Dependencies_should_be_accessible()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            IDependency dependency = fixture.Dependency<IDependency>();

            Assert.NotNull(dependency);
        }

        [Fact]
        public void Mocked_dependencies_should_be_accessible()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            Mock<IDependency> disp = fixture.Mocked<IDependency>();

            Assert.NotNull(disp);
        }

        [Fact]
        public void ResetSubject_should_give_another_instance_of_type()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            ClassWithDependencies instance1 = fixture.Subject;

            fixture.ResetSubject();

            ClassWithDependencies instance2 = fixture.Subject;

            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void ResetSubject_should_have_different_mock_dependencies()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            var origDependency = fixture.Mocked<IDependency>();

            fixture.ResetSubject();

            Assert.NotSame(origDependency, fixture.Mocked<IDependency>());
        }

        [Fact]
        public void ResetSubject_should_allow_a_different_config_to_be_passed()
        {
            var fixture = new AutoMoqTestFixture<Apple>();

            var looseConfig = new Config { MockBehavior = MockBehavior.Loose };
            fixture.ResetSubject(looseConfig);

            fixture.Subject.DoSomething(); // expecting no error

            var strictConfig = new Config { MockBehavior = MockBehavior.Strict };
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
            Assert.True(errorHit);
        }

        [Fact]
        public void Mocker_should_be_set()
        {
            var fixture = new AutoMoqTestFixture<ClassWithDependencies>();

            Assert.NotNull(fixture.Mocker);
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