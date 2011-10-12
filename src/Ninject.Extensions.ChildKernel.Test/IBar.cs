namespace Ninject.Extensions.ChildKernel
{
    /// <summary>
    /// Another test interface
    /// </summary>
    public interface IBar
    {
        /// <summary>
        /// Gets the name of the instance.
        /// </summary>
        /// <value>The name of the instance.</value>
        string Name { get; }

        /// <summary>
        /// Gets the number of activations.
        /// </summary>
        /// <value>The activation count.</value>
        int ActivationCount { get; }
    }
}