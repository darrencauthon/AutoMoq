using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;

namespace AutoMoq.Tests
{
    public class RandomValueTests
    {
        [TestFixture]
        public class IntegerTests
        {
            [SetUp]
            public void Setup()
            {
                mocker = new AutoMoqer();
            }

            private AutoMoqer mocker;

            [Test]
            public void It_should_return_an_integer_when_asked()
            {
                mocker.GetInt("x")
                    .ShouldBeInRange(int.MinValue, int.MaxValue);
            }

            [Test]
            public void It_should_return_different_values_somehow()
            {
                var list = new List<int>();
                foreach (var _ in Enumerable.Range(1, 100))
                {
                    var value = (new AutoMoqer()).GetInt("x");
                    if (list.Contains(value) == false)
                        list.Add(value);
                }
                list.Count.ShouldBeInRange(2, int.MaxValue);
            }

            [Test]
            public void It_should_return_the_same_value_with_the_same_key()
            {
                var list = new List<int>();
                foreach (var _ in Enumerable.Range(1, 100))
                {
                    var value = mocker.GetInt("x");
                    if (list.Contains(value) == false)
                        list.Add(value);
                }
                list.Count.ShouldEqual(1);
            }

            [Test]
            public void It_should_return_different_values_for_different_keys()
            {
                var list = new List<int>
                {
                    mocker.GetInt("x"),
                    mocker.GetInt("y"),
                    mocker.GetInt("z"),
                    mocker.GetInt("a"),
                    mocker.GetInt("b"),
                    mocker.GetInt("c"),
                    mocker.GetInt("d")
                };
                list.GroupBy(x => x).Count().ShouldEqual(7);
            }

            [Test]
            public void It_should_not_let_duplicate_values_in_the_dictionary()
            {
                var list = new Dictionary<string, int> {["x"] = 4};

                mocker.SetList(list);
            }

        }
    }
}