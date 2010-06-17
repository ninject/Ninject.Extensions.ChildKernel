//-------------------------------------------------------------------------------
// <copyright file="ChildKernelTest.cs" company="bbv Software Services AG">
//   Copyright (c) 2008 bbv Software Services AG
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
    using NUnit.Framework;

    /// <summary>
    /// Tests the implementation of <see cref="ChildKernel"/>.
    /// </summary>
    [TestFixture]
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
        private IKernel testee;

        /// <summary>
        /// The parent kernel.
        /// </summary>
        private IKernel parentKernel;

        /// <summary>
        /// A test interface.
        /// </summary>
        private interface IFoo
        {
            /// <summary>
            /// Gets the injected bar object.
            /// </summary>
            /// <value>The injected bar object.</value>
            IBar Bar { get; }

            /// <summary>
            /// Gets the name of the instance.
            /// </summary>
            /// <value>The name of the instance.</value>
            string Name { get; }
        }

        /// <summary>
        /// Another test interface
        /// </summary>
        private interface IBar
        {
            /// <summary>
            /// Gets the name of the instance.
            /// </summary>
            /// <value>The name of the instance.</value>
            string Name { get; }
        }
        
        /// <summary>
        /// Sets up the testee and its parent kernel.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.parentKernel = new StandardKernel();
            this.testee = new ChildKernel(this.parentKernel);
        }

        /// <summary>
        /// All known dependencies the are resolved on child kernel.
        /// </summary>
        [Test]
        public void DependenciesAreResolvedOnChildKernelIfPossible()
        {
            this.parentKernel.Bind<IFoo>().To<Foo>().WithConstructorArgument("name", ParentFooName);
            this.parentKernel.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ParentBarName);
            this.testee.Bind<IFoo>().To<Foo>().WithConstructorArgument("name", ChildFooName);
            this.testee.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ChildBarName);

            var foo = this.testee.Get<IFoo>();

            Assert.AreEqual(ChildFooName, foo.Name);
            Assert.AreEqual(ChildBarName, foo.Bar.Name);
        }

        /// <summary>
        /// All unknown dependencies are requested on the parent.
        /// </summary>
        [Test]
        public void DependenciesAreResolvedOnTheParentKernelIfMissing()
        {
            this.parentKernel.Bind<IFoo>().To<Foo>().WithConstructorArgument("name", ParentFooName);
            this.parentKernel.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ParentBarName);
            this.testee.Bind<IFoo>().To<Foo>().WithConstructorArgument("name", ChildFooName);

            var foo = this.testee.Get<IFoo>();

            Assert.AreEqual(ChildFooName, foo.Name);
            Assert.AreEqual(ParentBarName, foo.Bar.Name);
        }

        /// <summary>
        /// Objects created by the parent kernel can not get objects from the child kernel even if
        /// they are requested on the child kernel.
        /// </summary>
        [Test]
        public void ParentKernelCannotAccessChildKernelObjects()
        {
            this.parentKernel.Bind<IFoo>().To<Foo>().WithConstructorArgument("name", ParentFooName);
            this.testee.Bind<IBar>().To<Bar>().WithConstructorArgument("name", ChildBarName);

            Assert.Throws<ActivationException>(() => this.testee.Get<IFoo>());
        }

        /// <summary>
        /// A test object.
        /// </summary>
        private class Foo : IFoo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Foo"/> class.
            /// </summary>
            /// <param name="bar">The injected bar object.</param>
            /// <param name="name">The name of the instance.</param>
            public Foo(IBar bar, string name)
            {
                this.Bar = bar;
                this.Name = name;
            }

            /// <summary>
            /// Gets the injected bar object.
            /// </summary>
            /// <value>The injected bar object.</value>
            public IBar Bar { get; private set; }

            /// <summary>
            /// Gets the name of the instance.
            /// </summary>
            /// <value>The name of the instance.</value>
            public string Name { get; private set; }
        }

        /// <summary>
        /// Another test object
        /// </summary>
        private class Bar : IBar
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Bar"/> class.
            /// </summary>
            /// <param name="name">The name of the instance.</param>
            public Bar(string name)
            {
                this.Name = name;
            }

            /// <summary>
            /// Gets the name of the instance.
            /// </summary>
            /// <value>The name of the instance.</value>
            public string Name { get; private set; }
        }
    }
}