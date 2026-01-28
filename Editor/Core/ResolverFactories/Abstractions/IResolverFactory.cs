using JetBrains.Annotations;

namespace EasyToolkit.Inspector.Editor
{
    public interface IResolverFactory<TResolver> where TResolver : IResolver
    {
        [CanBeNull] TResolver CreateResolver(IElement element);
    }
}
