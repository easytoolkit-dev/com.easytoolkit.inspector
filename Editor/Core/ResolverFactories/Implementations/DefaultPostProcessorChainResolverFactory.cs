using EasyToolkit.Core;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Default implementation of <see cref="IPostProcessorChainResolverFactory"/>
    /// </summary>
    public class DefaultPostProcessorChainResolverFactory : IPostProcessorChainResolverFactory
    {
        /// <summary>
        /// Creates a post processor chain resolver for the specified element
        /// </summary>
        /// <param name="element">The element to create the resolver for</param>
        /// <returns>The created post processor chain resolver</returns>
        public IPostProcessorChainResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IPostProcessorChainResolver));
            if (resolverType != null)
            {
                var resolver = (IPostProcessorChainResolver)ResolverUtility.RentResolver(resolverType);
                resolver.Element = element;
                return resolver;
            }

            return null;
        }
    }
}
