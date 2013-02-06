namespace Ninject.Extensions.ChildKernel
{
    /// <summary>
    /// A test class
    /// </summary>
    public class Baz
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Baz"/> class.
        /// </summary>
        public Baz()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Baz"/> class.
        /// </summary>
        /// <param name="bar">The injected bar object.</param>
        public Baz(IBar bar)
        {
            this.Bar = bar;
        }

        /// <summary>
        /// Gets the injected bar object.
        /// </summary>
        /// <value>The injected bar object.</value>
        public IBar Bar { get; private set; }
    }
}