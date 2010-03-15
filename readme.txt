AutoMoqer is an "auto-mocking" container that automatically creates any fake objects that are necessary to instantiate the class under test. It's very similar to an IoC container in that it has a generic Resolve method that automatically fills in all dependencies, but unlike an IoC container it resolves the dependencies with mock objects.  This can make your testing cleaner by allowing you to only write code for the mock objects that are relevant to your specific test.

This tool uses Moq, my favorite .Net mocking framework, for all mocks.


Example code:


var mocker = new AutoMoqer();

mocker.GetMock<IDataDependency>()
   .Setup(x => x.GetData())
   .Returns("TEST DATA");

var classToTest = mocker.Resolve<ClassToTest>();

classToTest.DoSomething();

mocker.GetMock<IDependencyToCheck>()
   .Setup(x=>x.CallMe("TEST"), Times.Once());
   

