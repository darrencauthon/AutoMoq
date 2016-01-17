using Moq;

namespace AutoMoq.Helpers
{
    public class AutoMoqTestFixture<T> where T : class
    {
        private T subject;

        /// <summary>
        ///     The current AutoMoqer
        /// </summary>
        public AutoMoqer Mocker { get; private set; } = new AutoMoqer();

        /// <summary>
        ///     The Class being tested in this Test Fixture.
        /// </summary>
        public T Subject
        {
            get { return subject ?? (subject = Mocker.Resolve<T>()); }
        }

        /// <summary>
        ///     A Mock dependency that was auto-injected into Subject
        /// </summary>
        /// <typeparam name="TMock"></typeparam>
        /// <returns></returns>
        public Mock<TMock> Mocked<TMock>() where TMock : class
        {
            return Mocker.GetMock<TMock>();
        }

        /// <summary>
        ///     A dependency that was auto-injected into Subject.  Implementation is a Moq object.
        /// </summary>
        /// <typeparam name="TDepend"></typeparam>
        /// <returns></returns>
        public TDepend Dependency<TDepend>() where TDepend : class
        {
            return Mocked<TDepend>().Object;
        }

        /// <summary>
        ///     Resets Subject instance.  A new instance will be created, with new depenencies auto-injected.
        ///     Call this from NUnit's [SetUp] method, if you want each of your tests in the fixture to have a fresh instance of
        ///     <typeparamref name="T" />
        /// </summary>
        public void ResetSubject()
        {
            Mocker = new AutoMoqer();
            subject = null;
        }
    }
}