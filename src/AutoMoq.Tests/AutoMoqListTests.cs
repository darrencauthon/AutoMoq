using System;
using AutoMoq.Helpers;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Should;

namespace AutoMoq.Tests
{
    [TestFixture]
    public class AutoMoqListTests
    {
        private AutoMoqer mocker;

        [SetUp]
        public void Setup()
        {
            mocker = new AutoMoqer();
        }

        [Test]
        public void Can_register_and_resolve_a_list_using_the_declared_type()
        {
            var dependency = new Dependency();
            IList<Dependency> list = new List<Dependency>();
            list.Add(dependency);

            mocker.SetInstance(list);
            var thing = mocker.Create<ThingThatHasDependencies>();

            thing.FindOne().ShouldBeSameAs(dependency);
        }

        [Test]
        public void Can_register_and_resolve_a_list_with_an_explicit_type_provided_to_SetInstance()
        {
            var dependency = new Dependency();
            var list = new List<Dependency>();
            list.Add(dependency);

            mocker.SetInstance<IList<Dependency>>(list);
            var thing = mocker.Create<ThingThatHasDependencies>();

            thing.FindOne().ShouldBeSameAs(dependency);
        }

        [Test]
        public void Cannot_figure_out_how_to_map_a_registered_concrete_list_to_an_interface()
        {
            var dependency = new Dependency();
            var list = new List<Dependency>();
            list.Add(dependency);

            mocker.SetInstance(list);
            var thing = mocker.Create<ThingThatHasDependencies>();

            thing.FindOne().ShouldBeNull();
        }
    }

    public class Dependency
    {
    }

    public class ThingThatHasDependencies {
        private IList<Dependency> innerList;

        public ThingThatHasDependencies(IList<Dependency> dependencies) {
            innerList = dependencies;
        }

        public Dependency FindOne() {
            if(innerList == null || innerList.Any() == false) {
              return null;
            }

            return innerList.FirstOrDefault();      
        }
    }
}
