namespace EasyToolkit.Inspector.Editor
{
    public static class StructureResolverFactory
    {
        public static IStructureResolver CreateResolver(IElement element)
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
