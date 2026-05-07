namespace EasyToolkit.Inspector.Editor
{
    public static class DrawerChainResolverFactory
    {
        public static IDrawerChainResolver CreateResolver(IElement element)
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
