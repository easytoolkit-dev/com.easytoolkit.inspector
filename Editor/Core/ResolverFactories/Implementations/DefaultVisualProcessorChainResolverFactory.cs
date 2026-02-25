using EasyToolkit.Core;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Default implementation of visual processor chain resolver factory.
    /// </summary>
    public class DefaultVisualProcessorChainResolverFactory : IVisualProcessorChainResolverFactory
    {
        /// <summary>
        /// Creates a visual processor chain resolver for the specified element.
        /// </summary>
        /// <param name="element">The element to create a resolver for.</param>
        /// <returns>A visual processor chain resolver instance, or null if no appropriate resolver is found.</returns>
        public IVisualProcessorChainResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IVisualProcessorChainResolver));
            if (resolverType != null)
            {
                var resolver = (IVisualProcessorChainResolver)ResolverUtility.RentResolver(resolverType);
                resolver.Element = element;
                return resolver;
            }

            return null;
        }
    }
}
