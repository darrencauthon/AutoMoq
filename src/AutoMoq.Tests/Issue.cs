using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoMoq.Tests
{
    [TestClass]
    public class Issue
    {
        [TestMethod]
        public void can_create_parent_object_when_setInstance_is_called_on_child()
        {
            var autoMoq = new AutoMoqer();

            var child = autoMoq.Create<Child>();
            autoMoq.SetInstance<IChild>(child);

            var parent = autoMoq.Create<Parent>();
            Assert.IsNotNull(parent);
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