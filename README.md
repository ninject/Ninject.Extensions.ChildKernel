# Ninject.Extensions.ChildKernel [![NuGet Version](http://img.shields.io/nuget/v/Ninject.Extensions.ChildKernel.svg?style=flat)](https://www.nuget.org/packages/Ninject.Extensions.ChildKernel/) [![NuGet Downloads](http://img.shields.io/nuget/dt/Ninject.Extensions.ChildKernel.svg?style=flat)](https://www.nuget.org/packages/Ninject.Extensions.ChildKernel/)
This Ninject extension allows that child kernels can be defined. A child kernel is a 
Ninject kernel that has a parent kernel. All requests that it cant resolve are passed
to the parent kernel.

Restrictions:
- Objects that are resolved by the parent kernel can not have any dependency to an object
  defined on a child kernel. This is by design. Otherwise it would be possible to access
  objects on another child kernel if the object is defined as singleton.
- The default behavior of Ninject that classes are bound to themself if not explicit still
  exists. But in this case this will be done by the top most parent. This means that
  this class can not have any dependency defined on a child kernel. I strongly suggest to
  have a binding for all objects that are resolved by ninject and not to use this default behavior.

Example:
```C#
public class Foo
{
  public Foo(IBar bar)
  {
    this.Bar = bar;
  }

  public IBar Bar { get; private set; }
}

public class Bar
{
}

var parentKernel = new StandardKernel();
parentKernel.Bind<Bar>().ToSelf().InSingletonScope();

var childKernel1 = new ChildKernel(this.parentKernel);
childKernel1.Bind<Foo>().ToSelf().InSingletonScope();

var childKernel2 = new ChildKernel(this.parentKernel);
childKernel2.Bind<Foo>().ToSelf().InSingletonScope();

var foo1 = childKernel1.Get<Foo>();
var foo2 = childKernel2.Get<Foo>();
var foo3 = childKernel1.Get<Foo>();
```
In this example foo1 and foo2 will be different instances. foo1 and foo3 is the same instance. And all share the same bar.

## Documentation
https://github.com/ninject/Ninject.Extensions.ChildKernel/wiki

## CI build status
[![Build Status](https://teamcity.bbv.ch/app/rest/builds/buildType:(id:bt23)/statusIcon)](http://teamcity.bbv.ch/viewType.html?buildTypeId=bt23&guest=1)
