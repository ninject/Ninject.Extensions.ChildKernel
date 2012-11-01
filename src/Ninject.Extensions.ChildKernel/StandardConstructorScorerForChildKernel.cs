using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Planning.Targets;
using Ninject.Selection.Heuristics;
using Ninject.Syntax;

namespace Ninject.Extensions.ChildKernel
{
    public class StandardConstructorScorerForChildKernel : StandardConstructorScorer
    {
        protected override bool BindingExists(Activation.IContext context, Planning.Targets.ITarget target)
        {
            if (target.HasDefaultValue)
                return true;

            var targetType = GetTargetType(target);
            var searchKernel = context.Kernel;
            while(searchKernel != null)
            {
                if (searchKernel.GetBindings(targetType).Any(b => !b.IsImplicit))
                    return true;
                if (searchKernel is IChildKernel)
                    searchKernel = (searchKernel as IChildKernel).ParentResolutionRoot as IKernel;
                else break;
            }

            return false;
        }

        private Type GetTargetType(ITarget target)
        {
            var targetType = target.Type;
            if (targetType.IsArray)
            {
                targetType = targetType.GetElementType();
            }

            if (targetType.IsGenericType && targetType.GetInterfaces().Any(type => type == typeof(IEnumerable)))
            {
                targetType = targetType.GetGenericArguments()[0];
            }

            return targetType;
        }
    }
}
