using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class DefaultAttributeResolverFactory : IAttributeResolverFactory
    {
        public IAttributeResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IAttributeResolver));
            if (resolverType != null)
            {
                var resolver = (IAttributeResolver)ResolverUtility.RentResolver(resolverType);
                resolver.Element = element;
                return resolver;
            }

            return null;
        }
    }
}
