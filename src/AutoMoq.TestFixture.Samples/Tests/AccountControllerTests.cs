using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AutoMoq.Helpers;
using AutoMoq.TestFixture.Samples.Code;
using Moq;
using NUnit.Framework;

namespace AutoMoq.TestFixture.Samples.Tests
{
    [TestFixture]
    public class AccountControllerTests : AutoMoqTestFixture<AccountController>
    {
        [SetUp]
        public void BeforeEachTest()
        {
            ResetSubject();
        }

        [Test]
        public void ShouldListAllAccountsFromRepository()
        {
            Mocked<IAccountRepository>().Setup(
                x => x.Find()).Returns(
                    new[] {new Account(), new Account()});

            ViewResult result = Subject.ListAllAccounts() as ViewResult;

            var model = result.ViewData.Model as IEnumerable<Account>;

            Assert.That(model.Count(), Is.EqualTo(2));

            Mocked<IAccountRepository>()
                .Verify(x => x.SomethingElse(), Times.Once());
        }

        [Test]
        public void ShouldShowTheErrorPageWhenRepositoryHasErrors()
        {
            Mocked<IAccountRepository>().Setup(
                    x => x.Find()).Throws(new Exception());

            ViewResult result = Subject.ListAllAccounts() as ViewResult;

            Assert.That(result.ViewName, Is.EqualTo("Error"));
        }
    }
}
