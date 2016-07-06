using System;
using Moq;
using Xunit;

//using NUnit.Framework;

namespace AutoMoq.Tests
{
    public class ConfigTests
    {
        //[TestFixture]
        public class MockBehaviorTests
        {
            [Fact]
            public void It_should_default_to_loose()
            {
                var config = new Config();
                //Assert.AreEqual(MockBehavior.Loose, config.MockBehavior);
                if (config.MockBehavior != MockBehavior.Loose)
                    throw new Exception("uh oh!");
            }
        }
    }
}