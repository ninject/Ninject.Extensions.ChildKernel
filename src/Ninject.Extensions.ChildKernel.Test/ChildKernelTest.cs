//-------------------------------------------------------------------------------
// <copyright file="ChildKernelTest.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 Software Services AG
//   Remo Gloor (remo.gloor@gmail.com)
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject.Extensions.ChildKernel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;

    using Xunit;
    
    /// <summary>
    /// Tests the implementation of <see cref="ChildKernel"/>.
    /// </summary>
    public class ChildKernelTest
    {
        /// <summary>
        /// Name parent's foo.
        /// </summary>
        private const string ParentFooName = "ParentFoo";

        /// <summary>
        /// Name of parent's bar.
        /// </summary>
        private const string ParentBarName = "ParentBar";

        /// <summary>
        /// Name of child's foo.
        /// </summary>
        private const string ChildFooName = "ChildFoo";

        /// <summary>
        /// Name of child's bar.
        /// </summary>
        private const string ChildBarName = "ChildBar";

        /// <summary>
        /// The object under test.
        /// </summary>
        private readonly IKernel testee;

        /// <summary>
        /// The parent kernel.
        /// </summary>
        private readonly IKernel parentKernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildKernelTest"/> class.
        /// </summary>
        public ChildKernelTest()
        {
            this.parentKernel = new StandardKernel();
            this.testee = new ChildKernel(this.parentKernel);
        }

        /// <summary>
        /// All known dependencies the are resolved on child kernel.
        /// </summary>
        [Fact]
        public void DependenciesAreResolvedOnChildKernelIfPossible()
        {
            this.parentKernel.Bind<IFoo>().To<Foo>().WithConstructorArgument("name", ParentFooName);
            this.parentKernel.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ParentBarName);
            this.testee.Bind<IFoo>().To<Foo>().WithConstructorArgument("name", ChildFooName);
            this.testee.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ChildBarName);

            var foo = this.testee.Get<IFoo>();

            foo.Name.Should().Be(ChildFooName);
            foo.Bar.Name.Should().Be(ChildBarName);
        }

        /// <summary>
        /// All unknown dependencies are requested on the parent.
        /// </summary>
        [Fact]
        public void DependenciesAreResolvedOnTheParentKernelIfMissing()
        {
            this.parentKernel.Bind<IFoo>().To<Foo>().WithConstructorArgument("name", ParentFooName);
            this.parentKernel.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ParentBarName);
            this.testee.Bind<IFoo>().To<Foo>().WithConstructorArgument("name", ChildFooName);

            var foo = this.testee.Get<IFoo>();

            foo.Name.Should().Be(ChildFooName);
            foo.Bar.Name.Should().Be(ParentBarName);
        }

        /// <summary>
        /// Objects created by the parent kernel can not get objects from the child kernel even if
        /// they are requested on the child kernel.
        /// </summary>
        [Fact]
        public void ParentKernelCannotAccessChildKernelObjects()
        {
            this.parentKernel.Bind<IFoo>().To<Foo>().WithConstructorArgument("name", ParentFooName);
            this.testee.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ChildBarName);

            Assert.Throws<ActivationException>(() => this.testee.Get<IFoo>());
        }

        /// <summary>
        /// Objects the that are activated on parent are not activated again by the child kernel.
        /// </summary>
        [Fact]
        public void ObjectsThatAreActivatedOnParentAreNotActivatedAgain()
        {
            this.parentKernel.Bind<Bar>().ToSelf().WithConstructorArgument("name", ParentBarName);
            this.testee.Bind<IBar>().ToMethod(ctx => ctx.Kernel.Get<Bar>());

            var bar = this.testee.Get<IBar>();

            bar.ActivationCount.Should().Be(1);
        }

        [Fact]
        public void ImplicitBindingsAreResolvedOnChildKernel()
        {
            this.parentKernel.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ParentBarName);
            this.testee.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ChildBarName);

            var foo = this.testee.Get<Foo>(new ConstructorArgument("name", string.Empty));

            foo.Bar.Name.Should().Be(ChildBarName);
        }
    
        [Fact]
        public void ImplicitBindingsAreResolvedOnChildKernelEvenIfImplicitBindingExistsOnParent()
        {
            this.parentKernel.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ParentBarName);
            this.testee.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ChildBarName);

            this.parentKernel.Get<Foo>(new ConstructorArgument("name", string.Empty));
            var foo = this.testee.Get<Foo>(new ConstructorArgument("name", string.Empty));

            foo.Bar.Name.Should().Be(ChildBarName);
        }
    
        [Fact]
        public void ImplicitBindingsFallbackToParentIncaseTheChildCantResolve()
        {
            this.parentKernel.Components.Add<IMissingBindingResolver, BarMissingBindingResolver>();

            var bar = this.testee.Get<IBar>();

            bar.Name.Should().Be("parent");
        }

        public class BarMissingBindingResolver : NinjectComponent, IMissingBindingResolver
        {
            private readonly IKernel kernel;

            public BarMissingBindingResolver(IKernel kernel)
            {
                this.kernel = kernel;
            }

            public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
            {
                if (request.Service == typeof(IBar))
                {
                    var binding = new Binding(request.Service);
                    var builder = new BindingBuilder<IBar>(binding, this.kernel, string.Empty);
                    builder.To<Bar>().WithConstructorArgument("name", "parent");
                    return new[] { binding };
                }

                return Enumerable.Empty<IBinding>();
            }
        }
    }
}