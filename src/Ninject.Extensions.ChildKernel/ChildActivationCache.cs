//-------------------------------------------------------------------------------
// <copyright file="ChildActivationCache.cs" company="bbv Software Services AG">
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
    using Ninject.Activation.Caching;
    using Ninject.Components;

    /// <summary>
    /// The activation cache of child kernels.
    /// </summary>
    public class ChildActivationCache : NinjectComponent, IActivationCache
    {
        /// <summary>
        /// The cache of the parent kernel.
        /// </summary>
        private readonly IActivationCache parentCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildActivationCache"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public ChildActivationCache(IKernel kernel)
        {
            this.parentCache = ((IChildKernel)kernel).ParentResolutionRoot.Get<IKernel>().Components.Get<IActivationCache>();
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
        }

        /// <summary>
        /// Adds an activated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        public void AddActivatedInstance(object instance)
        {
            this.parentCache.AddActivatedInstance(instance);
        }

        /// <summary>
        /// Adds an deactivated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        public void AddDeactivatedInstance(object instance)
        {
            this.parentCache.AddDeactivatedInstance(instance);
        }

        /// <summary>
        /// Determines whether the specified instance is activated.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified instance is activated; otherwise, <c>false</c>.
        /// </returns>
        public bool IsActivated(object instance)
        {
            return this.parentCache.IsActivated(instance);
        }

        /// <summary>
        /// Determines whether the specified instance is deactivated.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified instance is deactivated; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDeactivated(object instance)
        {
            return this.parentCache.IsDeactivated(instance);
        }
    }
}