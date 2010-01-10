using AutoMoq.Unity;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoMoq.Tests.Unity
{
    [TestClass]
    public class AutoMockingContainerExtensionTests
    {
        [TestMethod]
        public void CanCreateAMockInterface()
        {
            var container = new UnityContainer();
            container.AddNewExtension<AutoMockingContainerExtension>();

            var action = container.Resolve<IDependency>();
        }

        [TestMethod]
        public void CanInstantiateAClassWithInterfaceDependency()
        {
            var container = new UnityContainer();
            container.AddNewExtension<AutoMockingContainerExtension>();

            var action = container.Resolve<Action>();
        }

        [TestMethod]
        public void RegisteringAnInstanceWillForgoTheMockObject()
        {
            var container = new UnityContainer();
            container.AddNewExtension<AutoMockingContainerExtension>();

            container.RegisterInstance<IDependency>(new DependencyImplementation());

            var dependency = container.Resolve<IDependency>();

            Assert.AreEqual(typeof (DependencyImplementation), dependency.GetType());
        }
    }

    public class Action
    {
        public Action(IDependency action, Today today)
        {
        }
    }

    public class Today
    {
        public Today(IDependency dependency)
        {
        }
    }

    public interface IDependency
    {
        int OnePlusOne();
    }

    public class DependencyImplementation : IDependency
    {
        public int OnePlusOne()
        {
            return -1;
        }
    }
}