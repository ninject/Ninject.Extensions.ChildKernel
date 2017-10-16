namespace Ninject.Extensions.ChildKernel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ninject.Activation.Caching;
    using Ninject.Components;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;

    public class ChildKernelComponentContainer : DisposableObject, IComponentContainer
    {
        private readonly IComponentContainer _parentComponentContainer;
        private readonly IComponentContainer _childComponentContainer;

        private readonly Type[] _immutableComponents;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildKernelComponentContainer"/> class.
        /// </summary>
        /// <param name="parentComponentContainer">The parent component container.</param>
        public ChildKernelComponentContainer(IComponentContainer parentComponentContainer)
        {
            if (parentComponentContainer == null)
            {
                throw new ArgumentNullException(nameof(parentComponentContainer));
            }
            
            _parentComponentContainer = parentComponentContainer;
            _childComponentContainer = new ComponentContainer();
            
            _childComponentContainer.Add<IActivationCache, ChildActivationCache>();
            _childComponentContainer.Add<IConstructorScorer, ChildKernelConstructorScorer>();
            _childComponentContainer.Add<ISelector, Selector>();
            _childComponentContainer.Kernel = Kernel;

            _immutableComponents = new[]
            {
                typeof(IActivationCache),
                typeof(IConstructorScorer),
                typeof(ISelector)
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildKernelComponentContainer"/> class.
        /// </summary>
        public ChildKernelComponentContainer(): this(new ComponentContainer())
        {
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><c>True</c> if called manually, otherwise by GC.</param>
        public override void Dispose(bool disposing)
        {
            _childComponentContainer.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Registers a component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        public void Add<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent
        {
            CheckImmutableComponent(typeof(TComponent));
            this._childComponentContainer.Add<TComponent, TImplementation>();
        }

        /// <summary>
        /// Registers a transient component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        public void AddTransient<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent
        {
            this._childComponentContainer.AddTransient<TComponent, TImplementation>();
        }

        /// <summary>
        /// Removes all registrations for the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        public void RemoveAll<T>()
            where T : INinjectComponent
        {
            this._childComponentContainer.RemoveAll<T>();
        }

        /// <summary>
        /// Removes the specified registration.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public void Remove<T, TImplementation>()
            where T : INinjectComponent
            where TImplementation : T
        {
            CheckImmutableComponent(typeof(T));
            this._childComponentContainer.Remove<T, TImplementation>();
        }

        /// <summary>
        /// Removes all registrations for the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        public void RemoveAll(Type component)
        {
            CheckImmutableComponent(component);
            this._childComponentContainer.RemoveAll(component);
        }

        /// <summary>
        /// Gets one instance of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The instance of the component.</returns>
        public T Get<T>()
            where T : INinjectComponent
        {
            return (T)this.Get(typeof(T));
        }

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A series of instances of the specified component.</returns>
        public IEnumerable<T> GetAll<T>()
            where T : INinjectComponent
        {
            return this.GetAll(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Gets one instance of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>The instance of the component.</returns>
        public object Get(Type component)
        {
            try
            {
                return this._childComponentContainer.Get(component);
            }
            catch (InvalidOperationException)
            {
                return this._parentComponentContainer.Get(component);
            }
        }

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>A series of instances of the specified component.</returns>
        public IEnumerable<object> GetAll(Type component)
        {
            return this._childComponentContainer.GetAll(component)
                .Concat(this._parentComponentContainer.GetAll(component));
        }

        /// <summary>
        /// The kernel
        /// </summary>
        public IKernel Kernel {
            get
            {
                return this._childComponentContainer.Kernel;
            }
            set
            { 
                this._childComponentContainer.Kernel = value;
            } 
        }

        private void CheckImmutableComponent(Type component)
        {
            if (_immutableComponents.Any(t => t == component))
            {
                throw new InvalidOperationException($"Component {nameof(component)} is immutable in childKernel.");
            }
        }
    }
}
