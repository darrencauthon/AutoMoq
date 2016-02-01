using Moq;
using NUnit.Framework;
using Should;

namespace AutoMoq.Tests
{
    [TestFixture]
    public class MockingBehaviorTests
    {
        [Test]
        public void It_should_support_loose_mocking()
        {
            var mocker = new AutoMoqer();
            var bar = mocker.Create<Bar>();
            bar.Throw();
            // an error should not be thrown, as this
            // is a loose mock
        }

        [Test]
        public void It_should_support_strict_mocking()
        {

            var config = new Config(){MockBehavior = MockBehavior.Strict};
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
            anErrorWasThrown.ShouldBeTrue();

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

        public interface IFoo
        {
            void Catch();
        }
        
    }
}