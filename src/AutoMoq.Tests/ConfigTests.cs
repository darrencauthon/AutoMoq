using Moq;
using Xunit;

namespace AutoMoq.Tests
{
    public class ConfigTests
    {
        public class MockBehaviorTests
        {
            [Fact]
            public void It_should_default_to_loose()
            {
                var config = new Config();
                Assert.Equal(MockBehavior.Loose, config.MockBehavior);
            }
        }
    }
}