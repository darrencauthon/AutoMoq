using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AutoMoq.Tests
{
    [TestClass]
    public class Issue
    {
        [TestMethod]
        public void can_create_parent_object_when_setInstance_is_called_on_child()
        {
            var autoMoq = new AutoMoqer();

            // the second line works... seems to be an issue calling Create twice?
            var child = autoMoq.Create<Child>();
            //var child = new Mock<IChild>().Object;
            autoMoq.SetInstance<IChild>(child);

            var parent = autoMoq.Create<Parent>();
            Assert.IsNotNull(parent);
        }

        [TestMethod]
        public void creating_the_child_twice()
        {
            var autoMoq = new AutoMoqer();

            autoMoq.Create<Child>();
            autoMoq.Create<Child>();
        }

        [TestMethod]
        public void resolving_the_same_type_twice_with_unity()
        {
            var container = new UnityContainer();

            var grandChild = new Mock<IGrandChild>().Object;
            container.RegisterInstance(grandChild);

            container.Resolve<Child>();
            container.Resolve<Child>();
        }

        [TestMethod]
        public void creating_the_child_once()
        {
            var autoMoq = new AutoMoqer();

            autoMoq.Create<Child>();
        }
    }

    public interface IParent
    {
    }

    public interface IChild
    {
    }

    public interface IGrandChild
    {
    }

    public class Parent : IParent
    {
        private readonly IChild _child;
        private readonly IGrandChild _grandChild;

        public Parent(IChild child, IGrandChild grandChild)
        {
            _child = child;
            _grandChild = grandChild;
        }
    }

    public class Child : IChild
    {
        private readonly IGrandChild _grandChild;

        public Child(IGrandChild grandChild)
        {
            _grandChild = grandChild;
        }
    }

    public class GrandChild : IGrandChild
    {
    }
}