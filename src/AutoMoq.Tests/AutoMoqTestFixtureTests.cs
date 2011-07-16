using System;
using Moq;
using NUnit.Framework;

namespace AutoMoq.Tests
{
    [TestFixture]
    public class AutoMoqTestFixtureTests
    {
        [Test]
        public void Subject_should_be_populated_after_construction()
        {
            var fixture = new AutoMoqTestFixture<SystemUnderTest>();

            Assert.That( fixture.Subject, Is.Not.Null);
        }

        [Test]
        public void Dependencies_should_be_accessible()
        {
            var fixture = new AutoMoqTestFixture<SystemUnderTest>();

            IDisposable dependency = fixture.Dependency<IDisposable>();
            
            Assert.That( dependency, Is.Not.Null );
        }

        [Test]
        public void Mocked_dependencies_should_be_accessible()
        {
            var fixture = new AutoMoqTestFixture<SystemUnderTest>();

            Mock<IDisposable> disp = fixture.Mocked<IDisposable>();
            
            Assert.That(disp, Is.Not.Null);
        }
    }

    public class SystemUnderTest
    {
        public IDisposable Disp { get; private set; }

        public SystemUnderTest(IDisposable disp)
        {
            Disp = disp;
        }
    }
}