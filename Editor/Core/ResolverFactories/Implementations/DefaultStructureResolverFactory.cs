using System.Collections.Generic;
using EasyToolkit.Core;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    public class DefaultStructureResolverFactory : IStructureResolverFactory
    {
        public IStructureResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IStructureResolver));
            if (resolverType != null)
            {
                var resolver = (IStructureResolver)ResolverUtility.RentResolver(resolverType);
                resolver.Element = element;
                return resolver;
            }

            return null;
        }
    }
}
