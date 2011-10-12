namespace Ninject.Extensions.ChildKernel
{
    /// <summary>
    /// A test interface.
    /// </summary>
    public interface IFoo
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
}