using Moq;
using NUnit.Framework;
using Should;

namespace AutoMoq.Tests
{
    public class ConfigTests
    {
        [TestFixture]
        public class MockBehaviorTests
        {
            [Test]
            public void It_should_default_to_loose()
            {
                var config = new Config();
                config.MockBehavior.ShouldEqual(MockBehavior.Loose);
            }
        }
    }
}