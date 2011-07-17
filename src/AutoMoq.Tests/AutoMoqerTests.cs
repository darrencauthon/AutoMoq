﻿using System;
using Moq;
using NUnit.Framework;
using Should;

namespace AutoMoq.Tests
{
    [TestFixture]
    public class AutoMoqerTests
    {
        private AutoMoqer mocker;

        [SetUp]
        public void Setup()
        {
            mocker = new AutoMoqer();
        }

        [Test]
        public void Can_resolve_a_concrete_class()
        {
            var concreteClass = mocker.Create<ConcreteClass>();
            concreteClass.ShouldNotBeNull();
        }

        [Test]
        public void Can_resolve_a_class_with_dependencies()
        {
            var concreteClass = mocker.Create<ClassWithDependencies>();
            concreteClass.ShouldNotBeNull();
        }

        [Test]
        public void Can_resolve_a_class_with_func_dependencies()
        {
            var concreteClass = mocker.Create<ClassWithFuncDependencies>();
            concreteClass.ShouldNotBeNull();
        }

        [Test]
        public void Can_test_against_a_func_dependency_as_if_it_were_not()
        {
            var concreteClass = mocker.Create<ClassWithFuncDependencies>();
            concreteClass.CallSomething();
            mocker.GetMock<IDependency>()
                .Verify(x => x.Something(), Times.Once());
        }
    }

    public class ConcreteClass
    {
    }

    public class ClassWithDependencies
    {
        public IDependency Dependency { get; set; }

        public ClassWithDependencies(IDependency dependency)
        {
            Dependency = dependency;
        }
    }

    public class ClassWithFuncDependencies
    {
        private readonly IDependency dependency;

        public ClassWithFuncDependencies(Func<IDependency> dependency)
        {
            this.dependency = dependency();
        }

        public IDependency GetTheDependency { get { return this.dependency; } }

        public void CallSomething()
        {
            dependency.Something();
        }
    }

    public interface IDependency
    {
        void Something();
    }
}