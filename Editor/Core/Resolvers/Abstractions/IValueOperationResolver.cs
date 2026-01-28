namespace EasyToolkit.Inspector.Editor
{
    public interface IValueOperationResolver : IResolver
    {
        IValueOperation GetOperation();
    }
}
