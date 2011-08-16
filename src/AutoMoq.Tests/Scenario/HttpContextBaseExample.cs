using System.Web;
using NUnit.Framework;

namespace AutoMoq.Tests.Scenario
{
    [TestFixture]
    public class Situation_where_the_wrong_http_context_base_is_passed_inTests
    {

        [Test]
        public void Reproduce_the_issue()
        {
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("foo", "bar"));

            var auto = new AutoMoqer();
            var httpRequestBase = auto.GetMock<HttpRequestBase>().Object;
            auto.GetMock<HttpContextBase>()
                .Setup(x => x.Request)
                .Returns(httpRequestBase);

            auto.GetMock<HttpRequestBase>()
                .SetupGet(x => x.Cookies)
                .Returns(cookies);

            var svc = auto.Create<ContextService>();
            var hascookie = svc.HasCookie("foo");
            Assert.IsTrue(hascookie);
        }

        public class ContextService
        {
            private readonly HttpContextBase _contextBase;

            public ContextService(HttpContextBase contextBase)
            {
                _contextBase = contextBase;
            }

            public bool HasCookie(string cookie)
            {
                var c = _contextBase.Request.Cookies[cookie];
                if (c == null)
                    return false;
                else
                    return true;
            }
        }
    }
}