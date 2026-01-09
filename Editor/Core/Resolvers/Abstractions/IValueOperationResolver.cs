namespace EasyToolKit.Inspector.Attributes.Editor
{
    public interface IValueOperationResolver : IResolver
    {
        IValueOperation GetOperation();
    }
}
