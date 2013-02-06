//-------------------------------------------------------------------------------
// <copyright file="ChildKernelConstructorScorer.cs" company="bbv Software Services AG">
//   Copyright (c) 2013 Software Services AG
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
    using Ninject.Activation;
    using Ninject.Planning.Targets;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Scores constructors by either looking for the existence of an injection marker
    /// attribute, or by counting the number of parameters including those defined on parent kernels.
    /// </summary>
    public class ChildKernelConstructorScorer : StandardConstructorScorer
    {
        /// <summary>
        /// Checkes whether a binding exists for a given target on the specified kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>Whether a binding exists for the target in the given context.</returns>
        protected override bool BindingExists(IKernel kernel, IContext context, ITarget target)
        {
            return base.BindingExists(kernel, context, target) || this.BindingExistsOnParentKernel(kernel, context, target);
        }

        /// <summary>
        /// Checkes whether a binding exists for a given target on the parent of the specified kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>Whether a binding exists for the target in the given context.</returns>
        private bool BindingExistsOnParentKernel(IKernel kernel, IContext context, ITarget target)
        {
            var childKernel = kernel as IChildKernel;
            if (childKernel != null)
            {
                var parentKernel = childKernel.ParentResolutionRoot as IKernel;
                return parentKernel != null && this.BindingExists(parentKernel, context, target);
            }

            return false;
        }
    }
}