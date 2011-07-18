using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

namespace AutoMoq
{
    public class AutoMoqTestFixture<T> where T: class
    {
        private AutoMoqer _moqer = new AutoMoqer();
        private T _subject;

        /// <summary>
        /// A Mock dependency that was auto-injected into Subject
        /// </summary>
        /// <typeparam name="TMock"></typeparam>
        /// <returns></returns>
        public Mock<TMock> Mocked<TMock>() where TMock : class
        {
            return _moqer.GetMock<TMock>();
        }

        /// <summary>
        /// A depenency that was auto-injected into Subject.  Implementation is a Moq object.
        /// </summary>
        /// <typeparam name="TDepend"></typeparam>
        /// <returns></returns>
        public TDepend Dependency<TDepend>() where TDepend : class
        {
            return Mocked<TDepend>().Object;
        }

        /// <summary>
        /// Resets Subject instance.  A new instance will be created, with new depenencies auto-injected.
        /// Call this from NUnit's [SetUp] method, if you want each of your tests in the fixture to have a fresh instance of <typeparamref name="T"/>
        /// </summary>
        public void ResetSubject()
        {
            _moqer = new AutoMoqer();
            _subject = null;
        }

        /// <summary>
        /// The Class being tested in this Test Fixture.
        /// </summary>
        /// 
        public T Subject
        {
            get { return _subject ?? (_subject = _moqer.Resolve<T>()); }
        }
    }
}
