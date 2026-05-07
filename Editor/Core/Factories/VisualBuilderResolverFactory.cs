namespace EasyToolkit.Inspector.Editor
{
    public static class VisualBuilderResolverFactory
    {
        /// <summary>
        /// Creates a visual builder resolver for the specified element.
        /// </summary>
        /// <param name="element">The element to create a resolver for.</param>
        /// <returns>A visual builder resolver instance, or null if no appropriate resolver is found.</returns>
        public static IVisualBuilderResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IVisualBuilderResolver));
            if (resolverType != null)
            {
                var resolver = (IVisualBuilderResolver)ResolverUtility.RentResolver(resolverType);
                resolver.Element = element;
                return resolver;
            }

            return null;
        }
    }
}
