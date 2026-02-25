using EasyToolkit.Core;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Default implementation of visual builder resolver factory.
    /// </summary>
    public class DefaultVisualBuilderResolverFactory : IVisualBuilderResolverFactory
    {
        /// <summary>
        /// Creates a visual builder resolver for the specified element.
        /// </summary>
        /// <param name="element">The element to create a resolver for.</param>
        /// <returns>A visual builder resolver instance, or null if no appropriate resolver is found.</returns>
        public IVisualBuilderResolver CreateResolver(IElement element)
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
