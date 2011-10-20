AutoMoqer is an "auto-mocking" container that automatically creates any fake objects that are necessary to instantiate the class under test. You can access those fakes through the mocker, or you can just ignore them if they're not important.

I wrote it for a few reason:

1.)  I was tired of having of my tests breaking the build whenever I added or removed a dependency to a class.
2.)  I was tired of managing instances of fakes in my unit tests, especially when they weren't relevant to test I was running.

Example code:


   var mocker = new AutoMoqer();

   mocker.GetMock<IDataDependency>()
      .Setup(x => x.GetData())
      .Returns("TEST DATA");

   var classToTest = mocker.Resolve<ClassToTest>();

   classToTest.DoSomething();

   mocker.GetMock<IDependencyToCheck>()
      .Setup(x=>x.CallMe("TEST"), Times.Once());
   

CallMe("TEST"), Times.Once());
   

