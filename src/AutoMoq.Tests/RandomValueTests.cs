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
        public class RandomIntegerTests
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
                mocker.Get<int>("x")
                    .ShouldBeInRange(int.MinValue, int.MaxValue);
            }

            [Test]
            public void It_should_return_different_values_somehow()
            {
                var list = new List<int>();
                foreach (var _ in Enumerable.Range(1, 100))
                {
                    var value = (int)(new AutoMoqer()).Get<int>("x");
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
                    var value = (int)mocker.Get<int>("x");
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
                    (int)mocker.Get<int>("x"),
                    (int)mocker.Get<int>("y"),
                    (int)mocker.Get<int>("z"),
                    (int)mocker.Get<int>("a"),
                    (int)mocker.Get<int>("b"),
                    (int)mocker.Get<int>("c"),
                    (int)mocker.Get<int>("d")
                };
                list.GroupBy(x => x).Count().ShouldEqual(7);
            }

            [Test]
            public void It_should_not_let_duplicate_values_in_the_dictionary()
            {
                var list = new Dictionary<string, int> {["x"] = 4};
                mocker.SetRandomValueDictionary(list);
            }

            [Test]
            public void It_should_not_let_values_overlap_between_keys()
            {
                mocker.SetNextRandomValue(5);
                var dictionary = new Dictionary<string, int> {["y"] = 5};
                mocker.SetRandomValueDictionary(dictionary);
                mocker.Get<int>("x").ShouldNotEqual(5);
            }

        }

        [TestFixture]
        public class RandomDoubleTests
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
                mocker.GetDouble("x")
                    .ShouldBeInRange(double.MinValue, double.MaxValue);
            }

            [Test]
            public void It_should_return_different_values_somehow()
            {
                var list = new List<double>();
                foreach (var _ in Enumerable.Range(1, 100))
                {
                    var value = (new AutoMoqer()).GetDouble("x");
                    if (list.Contains(value) == false)
                        list.Add(value);
                }
                list.Count.ShouldBeInRange(2, int.MaxValue);
            }

            [Test]
            public void It_should_return_the_same_value_with_the_same_key()
            {
                var list = new List<double>();
                foreach (var _ in Enumerable.Range(1, 100))
                {
                    var value = mocker.GetDouble("x");
                    if (list.Contains(value) == false)
                        list.Add(value);
                }
                list.Count.ShouldEqual(1);
            }

            [Test]
            public void It_should_return_different_values_for_different_keys()
            {
                var list = new List<double>
                {
                    mocker.GetDouble("x"),
                    mocker.GetDouble("y"),
                    mocker.GetDouble("z"),
                    mocker.GetDouble("a"),
                    mocker.GetDouble("b"),
                    mocker.GetDouble("c"),
                    mocker.GetDouble("d")
                };
                list.GroupBy(x => x).Count().ShouldEqual(7);
            }

            [Test]
            public void It_should_not_let_duplicate_values_in_the_dictionary()
            {
                var list = new Dictionary<string, double> {["x"] = 4};
                mocker.SetRandomValueDictionary(list);
            }

            [Test]
            public void It_should_not_let_values_overlap_between_keys()
            {
                mocker.SetNextRandomValue(5);
                var dictionary = new Dictionary<string, double> {["y"] = 5};
                mocker.SetRandomValueDictionary(dictionary);
                mocker.GetDouble("x").ShouldNotEqual(5);
            }

        }
    }
}