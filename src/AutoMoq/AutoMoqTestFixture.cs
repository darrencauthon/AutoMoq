using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

namespace AutoMoq
{
    public class AutoMoqTestFixture<T> where T: class
    {
        public T Subject { get; private set; }

        private readonly AutoMoqer _moqer = new AutoMoqer();
        
        public AutoMoqTestFixture()
        {
            Subject = _moqer.Resolve<T>();
        }

        public TDepend Dependency<TDepend>() where TDepend: class
        {
            return Mocked<TDepend>().Object;
        }

        public Mock<TMock> Mocked<TMock>() where TMock : class
        {
            return _moqer.GetMock<TMock>();
        }
    }
}
