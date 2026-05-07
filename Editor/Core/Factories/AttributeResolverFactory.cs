namespace EasyToolkit.Inspector.Editor
{
    public static class AttributeResolverFactory
    {
        public static IAttributeResolver CreateResolver(IElement element)
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
