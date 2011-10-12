namespace Ninject.Extensions.ChildKernel
{
    /// <summary>
    /// A test object.
    /// </summary>
    public class Foo : IFoo
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
}