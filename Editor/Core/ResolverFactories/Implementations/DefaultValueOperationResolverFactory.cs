using EasyToolkit.Core;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    public class DefaultValueOperationResolverFactory : IValueOperationResolverFactory
    {
        public IValueOperationResolver CreateResolver(IElement element)
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
