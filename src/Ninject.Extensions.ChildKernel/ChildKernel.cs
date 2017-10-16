//-------------------------------------------------------------------------------
// <copyright file="ChildKernel.cs" company="bbv Software Services AG">
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
    using System.Collections.Generic;
    using Ninject;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Components;
    using Ninject.Injection;
    using Ninject.Modules;
    using Ninject.Planning;
    using Ninject.Planning.Bindings.Resolvers;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;
    using Ninject.Syntax;
    using System;

    /// <summary>
    /// This is a kernel with a parent kernel. Any binding that can not be resolved by this kernel is forwarded to the
    /// parent.
    /// </summary>
    public class ChildKernel : KernelBase, IChildKernel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildKernel"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="modules">The modules.</param>
        public ChildKernel(IResolutionRoot parent, params INinjectModule[] modules)
            : base(CreateComponentContainerOfStandartKernel(), new NinjectSettings(), modules)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }
            
            ParentResolutionRoot = parent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildKernel"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="modules">The modules.</param>
        public ChildKernel(IResolutionRoot parent, INinjectSettings settings, params INinjectModule[] modules)
            : base(CreateComponentContainerOfStandartKernel(settings), settings, modules)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }
            
            ParentResolutionRoot = parent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildKernel"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="components">The components.</param>
        /// <param name="modules">The modules.</param>
        public ChildKernel(
            IResolutionRoot parent,
            INinjectSettings settings,
            IComponentContainer components,
            params INinjectModule[] modules)
            : base(new ChildKernelComponentContainer(components), settings, modules)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }
            
            ParentResolutionRoot = parent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildKernel"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="modules">The modules.</param>
        public ChildKernel(IKernel parent, params INinjectModule[] modules)
            : base(new ChildKernelComponentContainer(parent.Components), parent.Settings, modules)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }
            
            ParentResolutionRoot = parent;
        }

        /// <summary>
        /// Gets the parent resolution root.
        /// </summary>
        /// <value>The parent  resolution root.</value>
        public IResolutionRoot ParentResolutionRoot { get; }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanResolve(IRequest request)
        {
            return base.CanResolve(request) || this.ParentResolutionRoot.CanResolve(request, true);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">if set to <c>true</c> implicit bindings are ignored.</param>
        /// <returns>
        ///     <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            return base.CanResolve(request, ignoreImplicitBindings) || this.ParentResolutionRoot.CanResolve(request, true);
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>
        /// An enumerator of instances that match the request.
        /// </returns>
        public override IEnumerable<object> Resolve(IRequest request)
        {
            if (base.CanResolve(request))
            {
                return base.Resolve(request);
            }

            if (this.ParentResolutionRoot.CanResolve(request, true))
            {
                return this.ParentResolutionRoot.Resolve(request);
            }

            try
            {
                return base.Resolve(request);
            }
            catch (ActivationException)
            {
                try
                {
                    return this.ParentResolutionRoot.Resolve(request);
                }
                catch (ActivationException)
                {
                }

                throw;
            }
        }

        protected override void AddComponents()
        {
        }

        protected override IKernel KernelInstance => this;

        /// <summary>
        /// Create container with components of <see cref="StandardKernel"/>
        /// </summary>
        /// <param name="settings">The settings.</param>
        private static IComponentContainer CreateComponentContainerOfStandartKernel(INinjectSettings settings = null)
        {
            var container = new ChildKernelComponentContainer();

            container.Add<IPlanner, Planner>();
            container.Add<IPlanningStrategy, ConstructorReflectionStrategy>();
            container.Add<IPlanningStrategy, PropertyReflectionStrategy>();
            container.Add<IPlanningStrategy, MethodReflectionStrategy>();
            container.Add<IInjectionHeuristic, StandardInjectionHeuristic>();
            container.Add<IPipeline, Pipeline>();

            if (settings != null && !settings.ActivationCacheDisabled)
            {
                container.Add<IActivationStrategy, ActivationCacheStrategy>();
            }

            container.Add<IActivationStrategy, PropertyInjectionStrategy>();
            container.Add<IActivationStrategy, MethodInjectionStrategy>();
            container.Add<IActivationStrategy, InitializableStrategy>();
            container.Add<IActivationStrategy, StartableStrategy>();
            container.Add<IActivationStrategy, BindingActionStrategy>();
            container.Add<IActivationStrategy, DisposableStrategy>();
            container.Add<IBindingResolver, StandardBindingResolver>();
            container.Add<IBindingResolver, OpenGenericBindingResolver>();
            container.Add<IMissingBindingResolver, DefaultValueBindingResolver>();
            container.Add<IMissingBindingResolver, SelfBindingResolver>();

            if (settings != null && !settings.UseReflectionBasedInjection)
            {
                container.Add<IInjectorFactory, DynamicMethodInjectorFactory>();
            }
            else
            {
                container.Add<IInjectorFactory, ReflectionInjectorFactory>();
            }

            container.Add<ICache, Cache>();
            container.Add<ICachePruner, GarbageCollectionCachePruner>();
            container.Add<IModuleLoader, ModuleLoader>();
            container.Add<IModuleLoaderPlugin, CompiledModuleLoaderPlugin>();
            container.Add<IAssemblyNameRetriever, AssemblyNameRetriever>();

            return container;
        }
    }
}
