using Automoqer.Helpers;
using Moq;
using Xunit;

namespace AutoMoq.Tests
{
    public class with_automoqer_Tests
    {
        private AutoMoqer mocker;

        public with_automoqer_Tests()
        {
            mocker = new AutoMoqer();
        }

        [Fact]
        public void Instantiating_the_automoqer_sets_the_static_mocker()
        {
            WithAutoMoqer.mocker = null;
            var test = new WithAutoMoqer();
            Assert.NotNull(WithAutoMoqer.mocker);
        }

        [Fact]
        public void A_config_option_can_be_provided_when_setting_up()
        {
            WithAutoMoqer.mocker = null;
            new WithAutoMoqer(new Config { MockBehavior = MockBehavior.Loose });

            WithAutoMoqer.Create<Test>().DoSomething(); // should not fail

            new WithAutoMoqer(new Config { MockBehavior = MockBehavior.Strict });
            var errorHit = false;
            try
            {
                WithAutoMoqer.Create<Test>().DoSomething(); // should fail
            }
            catch
            {
                errorHit = true;
            }
            Assert.True(errorHit);
        }

        [Fact]
        public void GetMock_returns_the_mock()
        {
            WithAutoMoqer.mocker = new AutoMoqer();

            Assert.Same(WithAutoMoqer.GetMock<IDependency>(), WithAutoMoqer.mocker.GetMock<IDependency>());
        }

        [Fact]
        public void Create_returns_the_class_resolved_from_automoqer()
        {
            WithAutoMoqer.mocker = new AutoMoqer();

            Assert.Same(WithAutoMoqer.Create<Test>().Dependency, WithAutoMoqer.GetMock<IDependency>().Object);
        }

        [Fact]
        public void SetInstance_sets_the_instance()
        {
            WithAutoMoqer.mocker = new AutoMoqer();

            var instance = new Mock<IDependency>().Object;
            WithAutoMoqer.SetInstance(instance);

            Assert.Same(WithAutoMoqer.Create<Test>().Dependency, instance);
        }

        public interface IDependency
        {
            void DoSomething();
        }

        public class Test
        {
            public readonly IDependency Dependency;

            public Test(IDependency Dependency)
            {
                this.Dependency = Dependency;
            }

            public void DoSomething()
            {
                Dependency.DoSomething();
            }
        }
    }
}