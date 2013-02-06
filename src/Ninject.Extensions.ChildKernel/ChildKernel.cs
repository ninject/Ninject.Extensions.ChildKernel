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
    using System;
    using System.Collections.Generic;
    using Ninject;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Modules;
    using Ninject.Selection.Heuristics;
    using Ninject.Syntax;

    /// <summary>
    /// This is a kernel with a parent kernel. Any binding that can not be resolved by this kernel is forwarded to the
    /// parent.
    /// </summary>
    public class ChildKernel : StandardKernel, IChildKernel
    {
        /// <summary>
        /// The parent kernel.
        /// </summary>
        private readonly IResolutionRoot parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildKernel"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="modules">The modules.</param>
        public ChildKernel(IResolutionRoot parent, params INinjectModule[] modules)
            : base(modules)
        {
            this.parent = parent;
            
            this.Components.RemoveAll<IActivationCache>();
            this.Components.Add<IActivationCache, ChildActivationCache>(); 
          
            this.Components.RemoveAll<IConstructorScorer>();
            this.Components.Add<IConstructorScorer, ChildKernelConstructorScorer>();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildKernel"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="modules">The modules.</param>
        public ChildKernel(IResolutionRoot parent, INinjectSettings settings, params INinjectModule[] modules)
            : base(settings, modules)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets the parent resolution root.
        /// </summary>
        /// <value>The parent  resolution root.</value>
        public IResolutionRoot ParentResolutionRoot
        {
            get
            {
                return this.parent;
            }
        }
        
        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanResolve(IRequest request)
        {
            return base.CanResolve(request) || this.parent.CanResolve(request, true);
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
            return base.CanResolve(request, ignoreImplicitBindings) || this.parent.CanResolve(request, true);
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

            if (this.parent.CanResolve(request, true))
            {
                return this.parent.Resolve(request);
            }

            try
            {
                return base.Resolve(request);
            }
            catch (ActivationException)
            {
                try
                {
                    return this.parent.Resolve(request);
                }
                catch (ActivationException)
                {
                }

                throw;
            }
        }
    }
}