namespace EasyToolKit.Inspector.Attributes.Editor
{
    public interface IPostProcessor : IHandler
    {
        PostProcessorChain Chain { get; set; }
        void Process();
    }
}
