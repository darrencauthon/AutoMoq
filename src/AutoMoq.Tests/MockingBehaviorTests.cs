using Moq;
using Xunit;

namespace AutoMoq.Tests
{
    public class MockingBehaviorTests
    {
        [Fact]
        public void It_should_support_loose_mocking()
        {
            var mocker = new AutoMoqer();
            var bar = mocker.Create<Bar>();
            bar.Throw();
            // an error should not be thrown, as this
            // is a loose mock
        }

        [Fact]
        public void It_should_support_strict_mocking()
        {

            var config = new Config() { MockBehavior = MockBehavior.Strict };
            var mocker = new AutoMoqer(config);

            var bar = mocker.Create<Bar>();

            var anErrorWasThrown = false;
            try
            {
                bar.Throw();
            }
            catch
            {
                anErrorWasThrown = true;
            }
            Assert.True(anErrorWasThrown);

        }

        [Fact]
        public void It_should_allow_a_mock_to_be_changed_on_a_mock_by_mock_basis()
        {
            var mocker = new AutoMoqer();

            mocker.GetMock<IFoo>(MockBehavior.Loose);
            mocker.GetMock<IFooFoo>(MockBehavior.Strict);

            var bar = mocker.Create<BarBar>();

            bar.ThrowFoo(); // this one is loose, so it should not throw

            var anErrorWasThrown = false;
            try
            {
                bar.ThrowFooFoo(); // this one is strict, so it should throw
            }
            catch
            {
                anErrorWasThrown = true;
            }
            Assert.True(anErrorWasThrown);


        }

        [Fact]
        public void It_should_allow_a_mock_to_be_changed_on_a_mock_by_mock_basis_case_2()
        {
            var mocker = new AutoMoqer();

            mocker.GetMock<IFoo>(MockBehavior.Strict);
            mocker.GetMock<IFooFoo>(MockBehavior.Loose);

            var bar = mocker.Create<BarBar>();

            bar.ThrowFooFoo(); // this one is loose, so it should not throw

            var anErrorWasThrown = false;
            try
            {
                bar.ThrowFoo(); // this one is strict, so it should throw

            }
            catch
            {
                anErrorWasThrown = true;
            }
            Assert.True(anErrorWasThrown);
        }

        public class Bar
        {
            private readonly IFoo foo;

            public Bar(IFoo foo)
            {
                this.foo = foo;
            }

            public void Throw()
            {
                foo.Catch();
            }
        }

        public interface IFooFoo
        {
            void Catch();
        }

        public class BarBar
        {
            private readonly IFoo foo;
            private readonly IFooFoo foofoo;

            public BarBar(IFoo foo, IFooFoo foofoo)
            {
                this.foo = foo;
                this.foofoo = foofoo;
            }

            public void ThrowFoo()
            {
                foo.Catch();
            }

            public void ThrowFooFoo()
            {
                foofoo.Catch();
            }
        }

        public interface IFoo
        {
            void Catch();
        }

    }
}