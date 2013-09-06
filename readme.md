# AutoMoq

AutoMoqer is an "auto-mocking" container that creates objects for you. Just tell it what class to create and it will create it.

## But how?  

It injects mocks as any undefined dependencies.

````c#
class NeatoRepository {
  public NeatoRepository(ISomething something){
    // ..
  }
}

var mocker = new AutoMoqer();

var neatoRepository = mocker.Create<NeatoRepository>();

// but what about ISomething?

mocker.GetMock<ISomething>(); // I was injected as ISomething
````

### But why?  

Let's pretend you did not use AutoMoq and you changed your dependencies:

````c#

// I wrote this code in my tests...
var neatoRepository = new NeatoRepository(null);

// ... then I changed my class...
class NeatoRepository {
  public NeatoRepository(ISomething something, ISomethingElse somethingElse){
    // ..
  }
}

// NOW I HAVE TO FIX ALL OTHER REFERENCES TO GET A BUILD
var neatoRepository = new NeatoRepository(null);
````

If you used AutoMoq, this could would always compile:

````c#
var neatoRepository = mocker.Create<NeatoRepository>();
````

Leaving you to just worry about how to change your logic, **not your syntax**.


### Another Example

The dependencies injected into the class you are testing can be accessed before and/or after you call Create.  Like so:

````c#


   var mocker = new AutoMoqer();

   mocker.GetMock<IDataDependency>()
      .Setup(x => x.GetData())
      .Returns("TEST DATA");

   var classToTest = mocker.Resolve<ClassToTest>();

   classToTest.DoSomething();

   mocker.GetMock<IDependencyToCheck>()
      .Setup(x=>x.CallMe("TEST"), Times.Once());

````

### That's It

It's a simple tool, but it can save a lot of headaches.
