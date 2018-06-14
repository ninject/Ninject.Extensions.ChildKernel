# Ninject.Extensions.ChildKernel

[![Build status](https://ci.appveyor.com/api/projects/status/lg793a8v8ldnlp25?svg=true)](https://ci.appveyor.com/project/Ninject/ninject-extensions-childkernel)
[![codecov](https://codecov.io/gh/ninject/Ninject.Extensions.ChildKernel/branch/master/graph/badge.svg)](https://codecov.io/gh/ninject/Ninject.Extensions.ChildKernel)
[![NuGet Version](http://img.shields.io/nuget/v/Ninject.Extensions.ChildKernel.svg?style=flat)](https://www.nuget.org/packages/Ninject.Extensions.ChildKernel/) 
[![NuGet Downloads](http://img.shields.io/nuget/dt/Ninject.Extensions.ChildKernel.svg?style=flat)](https://www.nuget.org/packages/Ninject.Extensions.ChildKernel/)

This Ninject extension allows that child kernels can be defined. A child kernel is a 
Ninject kernel that has a parent kernel. All requests that it can't resolve are passed
to the parent kernel.

## Getting Started
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

In this example `foo1` and `foo2` will be different instances. 
`foo1` and `foo3` are the same instance. 
And all share the same `bar`.

## Note
- Objects that are resolved by the parent kernel can not have any dependency to an object
  defined on a child kernel. This is by design. Otherwise it would be possible to access
  objects on another child kernel if the object is defined as singleton.
- Since version 3.0, implicit bindngs are resolved on the child kernel and not the parent 
  kernel anymore.

## Documentation
https://github.com/ninject/Ninject.Extensions.ChildKernel/wiki