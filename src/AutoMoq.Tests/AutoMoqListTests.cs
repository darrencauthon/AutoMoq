using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoMoq.Tests
{
    public class AutoMoqListTests
    {
        private AutoMoqer mocker;

        public AutoMoqListTests()
        {
            mocker = new AutoMoqer();
        }

        [Fact]
        public void Can_register_and_resolve_a_list_using_the_declared_type()
        {
            var dependency = new Dependency();
            IList<Dependency> list = new List<Dependency>();
            list.Add(dependency);

            mocker.SetInstance(list);
            var thing = mocker.Create<ThingThatHasDependencies>();

            Assert.Same(thing.FindOne(), dependency);
        }

        [Fact]
        public void Can_register_and_resolve_a_list_with_an_explicit_type_provided_to_SetInstance()
        {
            var dependency = new Dependency();
            var list = new List<Dependency>();
            list.Add(dependency);

            mocker.SetInstance<IList<Dependency>>(list);
            var thing = mocker.Create<ThingThatHasDependencies>();

            Assert.Same(thing.FindOne(), dependency);
        }
    }

    public class Dependency
    {
    }

    public class ThingThatHasDependencies
    {
        private IList<Dependency> innerList;

        public ThingThatHasDependencies(IList<Dependency> dependencies)
        {
            innerList = dependencies;
        }

        public Dependency FindOne()
        {
            if (innerList == null || innerList.Any() == false)
            {
                return null;
            }

            return innerList.FirstOrDefault();
        }
    }
}
