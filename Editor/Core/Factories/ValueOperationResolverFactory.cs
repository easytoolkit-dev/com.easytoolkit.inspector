namespace EasyToolkit.Inspector.Editor
{
    public static class ValueOperationResolverFactory
    {
        public static IValueOperationResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IValueOperationResolver));
            if (resolverType != null)
            {
                var resolver = (IValueOperationResolver)ResolverUtility.RentResolver(resolverType);
                resolver.Element = element;
                return resolver;
            }

            return null;
        }
    }
}
