using EasyToolkit.Core;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    public class DefaultDrawerChainResolverFactory : IDrawerChainResolverFactory
    {
        public IDrawerChainResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IDrawerChainResolver));
            if (resolverType != null)
            {
                var resolver = (IDrawerChainResolver)ResolverUtility.RentResolver(resolverType);
                resolver.Element = element;
                return resolver;
            }

            return null;
        }
    }
}
