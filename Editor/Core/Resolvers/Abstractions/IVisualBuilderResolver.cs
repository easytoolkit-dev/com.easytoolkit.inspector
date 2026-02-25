namespace EasyToolkit.Inspector.Editor
{
    public interface IVisualBuilderResolver : IResolver
    {
        IVisualBuilder GetVisualBuilder();
    }
}
